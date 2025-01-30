﻿using AspNetCoreGeneratedDocument;
using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using FribergRentalCars.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace FribergRentalCars.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAccountRepository _accRepo;
        private readonly IAdressRepository _adrRepo;
        private readonly IBookingRepository _bookRepo;
        private readonly ICarRepository _carRepo;
        private readonly IUserRepository _userRepo;
        
        public AdminController(IAccountRepository accRepo, IUserRepository userRepo,
            ICarRepository carRepo, IBookingRepository bookRepo, IAdressRepository adressRepo)
        {
            this._accRepo = accRepo;
            this._adrRepo = adressRepo;
            this._bookRepo = bookRepo;
            this._carRepo = carRepo;
            this._userRepo = userRepo; 
        }

        #region // ADMIN

        // GET: AdminController/Details/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> Details()
        {
            try
            {
                var getAccount = HttpContext.Session.GetInt32("accountID");
                if(getAccount != null)
                {
                    var account = await _accRepo.GetWithAdressAsync((int)getAccount!);
                    return View(account);
                }
                else
                {
                    NotFound($"No Account with ID: {getAccount}");
                }
                
            }
            catch
            {
                NotFound($"Session ID not found");
            }
            return View();
        }

        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // POST: AdminController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                // Fetch user by UserName
                var user = await _userRepo.GetUserByUserNameAsync(loginVM.EmailOrUserName);

                // If incorrect UserName, let's try the Email option
                if (user == null)
                {
                    try
                    {
                        user = await _userRepo.GetUserByEmailAsync(loginVM.EmailOrUserName);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Användaren finns inte!");
                    }
                }

                // Password check
                if (loginVM.Password == user.Password)
                {
                    HttpContext.Session.SetInt32("accountID", user.AccountId);
                    HttpContext.Session.SetString("user", user.UserName);
                    // Control check if Admin
                    if (user.IsAdmin)
                    {
                        HttpContext.Session.SetInt32("isAdmin", 1);
                        return RedirectToAction(nameof(Details));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Lösenordet eller användarnamn/ email är inkorrekt.");
                }
            }
            return View(loginVM);
        }
        #endregion

        #region // BOOKINGS
        
        // GET: AdminController/GetAllBookings/
        public async Task<ActionResult> AllBookings()
        {
            var allBookings = await _bookRepo.GetAllAsync();

            List<AllBookingsViewModel> BookingsList = new List<AllBookingsViewModel>();

            foreach(var item in allBookings)
            {
                try
                {
                    item.Car = await _carRepo.GetIdByAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("item.Car", "Bilobjektet finns inte.");
                }

                var account = await _accRepo.GetIdByAsync((int)item.AccountId!);
                if(account == null)
                {
                    ModelState.AddModelError("", "Kontot hittades inte!");
                }

                var listObjekt = new AllBookingsViewModel
                {
                    BookingId = item.BookingId,
                    AccountId = item.AccountId,
                    //Email = account.Email,
                    CarId = item.CarId,
                    Car = item.Car,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    TotalCost = item.TotalCost,
                    IsFinished = item.IsFinished
                };
                               
                if(account == null || string.IsNullOrEmpty(account.Email))
                {
                    listObjekt.Email = "--GDPR--";
                }
                else
                {
                    listObjekt.Email = account.Email;
                }
               
                // Check if a booking is due. If so set to finished
                if(CheckFinished(item))
                {
                    await _bookRepo.UpdateAsync(item);
                }
                
                BookingsList.Add(listObjekt);
            }
            return View(BookingsList);
        }

        #endregion


        #region // CARS

        // GET: AdminController/CreateCar/
        [AdminAuthorizationFilter]
        public ActionResult CreateCar()
        {
            return View();
        }

        // POST: AdminController/CreateCar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> CreateCar(Car car)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _carRepo.AddAsync(car);
                    return RedirectToAction(nameof(ListAllCars));
                }
                catch
                {
                    ModelState.AddModelError("", "Databas fel: Objektet kunde inte adderas");
                }
            }
            return View(car);
        }

        // GET: AdminController/EditCar/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditCar(int carId)
        {
            if(carId == null)
            {
                ModelState.AddModelError("", $"Ingen bil med ID: {carId} funnen.");
            }
            var car = await _carRepo.GetIdByAsync(carId);
            if(car == null)
            {
                ModelState.AddModelError("", $"Bil ID korrekt, men bil objekt saknas.");
            }
            return View(car);
        }

        // POST: AdminController/EditCar/5,Car
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditCar(int carId, Car car)
        {
            if(carId != car.CarId)
            {
                ModelState.AddModelError("", $"modell ID: {carId} och bil ID: {car.CarId} ej samma.");
            }

            if(ModelState.IsValid)
            {
                try
                {
                    await _carRepo.UpdateAsync(car);
                    return RedirectToAction(nameof(ListAllCars));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Bilobjektet kunde inte uppdateras i databasen.");
                    NotFound(car);
                }
            }
            return View(car);
        }

        // GET: AdminController/ListAllCars
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ListAllCars()
        {
            var allCars = await _carRepo.GetAllAsync();
            return View(allCars);
        }

        #endregion


        #region // ACCOUNTS

        // GET: AdminController/CreateAccount
        [AdminAuthorizationFilter]
        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> CreateAccount(RegisterViewModel regVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await _userRepo.UserNameAvailaibility(regVM.User.UserName))
                    {
                        ModelState.AddModelError("", "Det här användarnamnet är redan registrerat.");
                        return View(regVM);
                    }
                    else
                    {
                        await _accRepo.AddAsync(regVM.Account);

                        regVM.User.AccountId = regVM.Account.AccountId;
                        await _userRepo.AddAsync(regVM.User);
                        return RedirectToAction(nameof(ListAllAccounts));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Det gick inte att skapa användare eller konto: {ex.Message}");
                }
            }
            return View(regVM);
        }
        


        // GET: AdminController/DeleteAccount/5  // TO DO
        [AdminAuthorizationFilter]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            if (userId == null)
            {
                NotFound();
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if(user == null)
            {
                NotFound(user);
            }

            var account = await _accRepo.GetIdByAsync(user.AccountId);
            if(account == null)
            {
                NotFound(account);
            }

            var adress = await _adrRepo.GetIdByAsync(account.AdressId);
            if(adress == null)
            {
                NotFound(adress);
            }

            var model = new EditAccountViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                IsAdmin = user.IsAdmin,
                
                AccountId = user.AccountId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,

                AdressId = account.AdressId,
                Street = adress.Street,
                PostalCode = adress.PostalCode,
                City = adress.City
            };
            return View("ConfirmDelete", model);
        }

        // POST: AdminController/DeleteAccount/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ConfirmDelete(int userId, EditAccountViewModel model)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if(user == null)
            {
                ModelState.AddModelError("", "Användaren finns inte!");
                return View(model);
            }

            var account = await _accRepo.GetIdByAsync(user.AccountId);
            if(account == null)
            {
                ModelState.AddModelError("", "Kontot finns inte!");
                return View(model);
            }
            var adress = await _adrRepo.GetIdByAsync(model.AdressId);
            if (adress == null)
            {
                ModelState.AddModelError("", "Adressen finns inte!");
                return View(model);
            }
            try
            {
                List<Booking> accountBookings = new List<Booking>
                    ( await _bookRepo.GetFinishedAccountBookings(account.AccountId) );
                
                //List<Booking> accountBookings = new List<Booking>(await _bookRepo.GetBookingsByAccountIdAsync(account.AccountId));
                foreach (var booking in accountBookings)
                {
                    //await _bookRepo.GetIdByAsync(booking.BookingId);
                    //booking.AccountId = null;
                    booking.EndDate = DateOnly.FromDateTime(DateTime.Today);
                    booking.IsFinished = true;
                    await _bookRepo.UpdateAsync(booking);
                }

                await _userRepo.DeleteAsync(user);

                await _adrRepo.DeleteAsync(adress);

                await _accRepo.DeleteAsync(account);

                return RedirectToAction(nameof(ListAllAccounts));
            }
            catch
            {
                ModelState.AddModelError("", "Det gick inte att radera användaren/ kontot");
                return View("DeleteUser", model);
            }
        }

        // GET: AdminController/EditAccount/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int userId)
        {
            if(userId == null)
            {
                return NotFound();
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }

            var account = await _accRepo.GetIdByAsync(user.UserId);
            if(account == null)
            {
                return NotFound();
            }

            var adress = await _adrRepo.GetIdByAsync(account.AccountId);
            if(adress == null)
            {
                return NotFound();
            }

            var model = new EditAccountViewModel
            {
                UserId = user.UserId,
                AccountId = user.AccountId,

                AdressId = account.AdressId,
                Street = adress.Street,
                PostalCode = adress.PostalCode,
                City = adress.City,

                FirstName = account.FirstName,
                LastName = account.LastName,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,

                UserName = user.UserName,
                Password = user.Password,
                IsAdmin = user.IsAdmin

            };
            return View(model);
        }

        // POST: AdminController/EditAccount/Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int userId, EditAccountViewModel model)
        {
            if(model == null)
            {
                ModelState.AddModelError("", "Konto-objektet finns inte.");
            }

            var user = await _userRepo.GetByIdAsync(userId);
            var account = await _accRepo.GetIdByAsync(user.UserId);
            var adress = await _adrRepo.GetIdByAsync(account.AccountId);

            var oldUserName = user.UserName;
            if(ModelState.IsValid)
            {
                if(model.UserName != oldUserName)
                {
                    if(await _userRepo.UserNameAvailaibility(model.UserName))
                    {
                        ModelState.AddModelError("", "Användarnamnet är redan taget!");
                        return View(model);
                    }
                    else
                    {
                        user.UserName = model.UserName;
                        oldUserName = model.UserName;
                    }
                }
                else
                {
                    // set User data
                    user.UserName = model.UserName;
                    user.Password = model.Password;
                    user.ConfirmPassword = model.Password;

                    // Set Account data
                    account.FirstName = model.FirstName;
                    account.LastName = model.LastName;
                    account.PhoneNumber = model.PhoneNumber;
                    account.Email = model.Email;

                    // Set Adress data
                    adress.Street = model.Street;
                    adress.PostalCode = model.PostalCode;
                    adress.City = model.City;
                }
                try
                {
                    await _userRepo.UpdateAsync(user);
                    await _accRepo.UpdateAsync(account);
                    await _adrRepo.UpdateAsync(adress);
                    return RedirectToAction(nameof(ListAllAccounts));
                }
                catch
                {
                    ModelState.AddModelError("", "Det blev något fel");
                } 
            }
            return View(model);
        }

        // GET: AdminController/ListAllAccounts
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ListAllAccounts()
        {
            var allUsers = await _userRepo.GetAllAsync();
            List<ListAllAccountsViewModel> accVMList = new List<ListAllAccountsViewModel>();

            foreach(var user in allUsers)
            {
                var account = await _accRepo.GetIdByAsync(user.AccountId);
                var newAccVM = new ListAllAccountsViewModel
                {
                    User = user,
                    Account = account,
                    Adress = account.Adress
                };
                accVMList.Add(newAccVM);
            }

            return View(accVMList);
        }

        #endregion

        #region // HELPER METHODS ------------------------------------------------------

        public bool CheckFinished(Booking booking)
        {
            if (booking.EndDate < DateOnly.FromDateTime(DateTime.Now) && booking.IsFinished == false)
            {
                booking.IsFinished = true;
                return true;
            }
            return false;
        }

        #endregion
    }
}
