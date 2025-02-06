using AspNetCoreGeneratedDocument;
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
using System.Security.Principal;

namespace FribergRentalCars.Controllers
{
    public class AdminController : BaseController
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
            ViewBag.UserName = HttpContext.Session.GetString("user");
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
                    HttpContext.Session.SetInt32("UserID", user.UserId);
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

        // GET: AdminController/AllBookings/
        [AdminAuthorizationFilter]
        public async Task<ActionResult> AllActiveBookings()
        {
            var allBookings = await _bookRepo.GetAllActiveAsync();

            List<AllBookingsViewModel> BookingsList = new List<AllBookingsViewModel>();

            foreach(var item in allBookings)
            {
                try
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("item.Car", "Bilobjektet finns inte.");
                }

                var account = await _accRepo.GetByIdAsync((int)item.AccountId!);
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

        // GET: AdminController/AllFinishedBookings/
        [AdminAuthorizationFilter]
        public async Task<ActionResult> AllFinishedBookings()
        {
            var allBookings = await _bookRepo.GetAllFinishedAsync();

            List<AllBookingsViewModel> BookingsList = new List<AllBookingsViewModel>();

            foreach (var item in allBookings)
            {
                try
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("item.Car", "Bilobjektet finns inte.");
                }

                var email = "";
                try
                {
                    var account = await _accRepo.GetByIdAsync((int)item.AccountId!);
                    if(account != null)
                    {
                        email = account.Email;
                    }
                }
                catch
                {
                    email = "--GDPR--";
                }

                var listObjekt = new AllBookingsViewModel
                {
                    BookingId = item.BookingId,
                    AccountId = item.AccountId,
                    Email = email,
                    CarId = item.CarId,
                    Car = item.Car,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    TotalCost = item.TotalCost,
                    IsFinished = item.IsFinished
                };

                // Check if a booking is due. If so set to finished
                if (CheckFinished(item))
                {
                    await _bookRepo.UpdateAsync(item);
                }

                BookingsList.Add(listObjekt);
            }
            return View(BookingsList);
        }

        // GET: AdminController/ActiveAccountBookings/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ActiveAccountBookings(int id)
        {
            var accBookings = await _bookRepo.GetActiveAccountBookings(id);
            // Retrieve User id for getting back to Edit Account properly (takes userId as parameter)
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserID = userId;
            // Retrieve UserName for listing bookings for account, properly
            ViewBag.UserName = await _userRepo.FindUserNameByIdAsync((int)userId!);

            foreach (var item in accBookings)
            {
                try
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("item.Car", "Bilobjektet finns inte.");
                }
            }
            return View(accBookings);
        }

        [AdminAuthorizationFilter]
        public async Task<ActionResult> CancelAccountBooking(int id)
        {
            try
            {
                var booking = await _bookRepo.GetByIdAsync(id);
                await _bookRepo.DeleteAsync(booking);
                return RedirectToAction("AllAccountBookings");
            }
            catch
            {
                ModelState.AddModelError("", "Kunde inte radera bokningen.");
                return NotFound(id);
                
            }
        }

        [AdminAuthorizationFilter]
        public async Task<ActionResult> CancelListBooking(int id)
        {
            try
            {
                var booking = await _bookRepo.GetByIdAsync(id);
                await _bookRepo.DeleteAsync(booking);
                return RedirectToAction("AllActiveBookings");
            }
            catch
            {
                ModelState.AddModelError("", "Kunde inte radera bokningen.");
                return NotFound(id);

            }
        }


        // GET: AdminController/FinishedAccountBookings/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> FinishedAccountBookings(int id)
        {
            var oldBookings = await _bookRepo.GetFinishedAccountBookings(id);
            // Retrieve User id for getting back to Edit Account properly (takes userId as parameter)
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserID = userId;
            // Retrieve UserName for listing bookings for account, properly
            ViewBag.UserName = await _userRepo.FindUserNameByIdAsync((int)userId!);

            foreach (var item in oldBookings)
            {
                try
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("item.Car", "Bilobjektet finns inte.");
                }
            }
            return View(oldBookings);
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
            var car = await _carRepo.GetByIdAsync(carId);
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

        // GET: AdminController/ListAllCars
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ListAllAvailableCars()
        {
            var allCars = await _carRepo.GetAllAvailableAsync();
            return View(allCars);
        }

        // GET: AdminController/ListAllCars
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ListAllUnAvailableCars()
        {
            var allCars = await _carRepo.GetAllUnAvailableAsync();
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

        // GET: AdminController/DeleteAccount
        [AdminAuthorizationFilter]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            if(id <= 0)
            {
                Console.WriteLine($"ID: {id} not found!");
                return NotFound(id);
            }

            var user = await _userRepo.GetByIdAsync(id);
            if(user == null)
            {
                Console.WriteLine($"No User with ID: {id} found!");
                return NotFound(user);
            }

            var account = await _accRepo.GetWithAdressAsync(user.AccountId);
            if(account == null)
            {
                Console.WriteLine($"No Account with User.AccountId: {user.AccountId} found!");
                return NotFound(user.AccountId);
            }
            else if(account.Adress == null)
            {
                Console.WriteLine($"No Adress with Account.AdressId: {account.AdressId} found!");
                return NotFound(account.Adress);
            }
            else
            {
                var model = new DeleteUserViewModel
                {
                    User = user,
                    Account = account,
                    Adress = account.Adress
                };

                return View(model);
            } 
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> DeleteConfirm(DeleteUserViewModel model)
        {
            try
            {
                List<Booking> accountBookings = new List<Booking>
                           (await _bookRepo.GetBookingsByAccountIdAsync(model.Account.AccountId));

                foreach (var booking in accountBookings)
                {
                    // If a booking is not finished, erase it
                    if(!booking.IsFinished)
                    {
                        await _bookRepo.DeleteAsync(booking);
                    }
                    // If a booking IS finished, set its AccountId to null, to apply GDPR
                    else
                    {
                        booking.AccountId = null;
                        await _bookRepo.UpdateAsync(booking);
                    }
                }

                await _userRepo.DeleteAsync(model.User);
                await _adrRepo.DeleteAsync(model.Adress);
                await _accRepo.DeleteAsync(model.Account);

                return RedirectToAction(nameof(ListAllAccounts));
            }
            catch
            {
                ModelState.AddModelError("", "Användaren kunde inte raderas");
                return RedirectToAction(nameof(ListAllAccounts));
            }
           
        }

        // GET: AdminController/EditAccount/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int id)
        {
            if(id <= 0)
            {
                return NotFound(id);
            }

            var user = await _userRepo.GetByIdAsync(id);
            if(user == null)
            {
                return NotFound(user);
            }

            var account = await _accRepo.GetByIdAsync(user.UserId);
            if(account == null)
            {
                return NotFound(account);
            }

            var adress = await _adrRepo.GetByIdAsync(account.AccountId);
            if(adress == null)
            {
                return NotFound(adress);
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

            // Set Session variable for getting back from Account Bookings view
            HttpContext.Session.SetInt32("UserId", user.UserId);
            return View(model);
        }

        // POST: AdminController/EditAccount/Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int id, EditAccountViewModel model)
        {
            if(model == null)
            {
                ModelState.AddModelError("", "Konto-objektet finns inte.");
            }

            var user = await _userRepo.GetByIdAsync(id);
            var account = await _accRepo.GetByIdAsync(user.UserId);
            var adress = await _adrRepo.GetByIdAsync(account.AccountId);

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
                var account = await _accRepo.GetByIdAsync(user.AccountId);
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
