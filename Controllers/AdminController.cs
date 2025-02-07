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
                item.Car = await _carRepo.GetByIdAsync(item.CarId);
                if (item.Car == null)
                {
                    TempData["ErrorMessage"] = $"Bil objektet med ID: {item.CarId} okänt.";
                    return View("ErrorPage", "Home");
                }

                var account = await _accRepo.GetByIdAsync((int)item.AccountId!);
                if (account == null)
                {
                    TempData["ErrorMessage"] = $"Användarkonto med ID: {item.AccountId} hittades inte.";
                    return View("ErrorPage", "Home");
                }
                
                var listObjekt = CreateBookingVM(item);

                // Check if GDPR is applied for Account
                listObjekt.Email = IsAccountGDPR(account, listObjekt);

                // Control check: Is Booking supposed to be finished?
                if (IsBookingFinished(item))
                    await _bookRepo.UpdateAsync(item);

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
                item.Car = await _carRepo.GetByIdAsync(item.CarId);
                if (item.Car == null)
                {
                    TempData["ErrorMessage"] = $"Bil objektet med ID: {item.CarId} okänt.";
                    return View("ErrorPage", "Home");
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
                                
                var listObjekt = CreateBookingVM(item, email);
                
                // Control check: Is Booking not set to finished? -do so
                if (IsBookingFinished(item))
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
            ViewBag.UserID = HttpContext.Session.GetInt32("tempUserID");
            // Retrieve UserName for listing bookings for account, properly
            ViewBag.UserName = HttpContext.Session.GetString("tempUserName");

            foreach (var item in accBookings)
            {
                item.Car = await _carRepo.GetByIdAsync(item.CarId);
                if (item.Car == null)
                {
                    TempData["ErrorMessage"] = $"Bil objektet med ID: {item.CarId} okänt.";
                    return View("ErrorPage", "Home");
                }
            }
            return View(accBookings);
        }

        [AdminAuthorizationFilter]
        public async Task<ActionResult> CancelAccountBooking(int id)
        {
            var booking = await _bookRepo.GetByIdAsync(id);
            if (booking == null)
            {
                TempData["ErrorMessage"] = $"Bokning med ID: {id} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }
            else
            {
                await _bookRepo.DeleteAsync(booking);
                return RedirectToAction("ActiveAccountBookings");
            }
        }

        // GET: AdminController/FinishedAccountBookings/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> FinishedAccountBookings(int id)
        {
            if (id <=0)
            {
                TempData["ErrorMessage"] = $"Användarkonto med ID: {id} okänt.";
                return View("ErrorPage", "Home");
            }

            var oldBookings = await _bookRepo.GetFinishedAccountBookings(id);

            // Retrieve User id for getting back to Edit Account properly (takes userId as parameter)
            ViewBag.UserID = HttpContext.Session.GetInt32("tempUserID");
            // Retrieve UserName for listing bookings for account, properly
            ViewBag.UserName = HttpContext.Session.GetString("tempUserName");

            foreach (var item in oldBookings)
            {
                item.Car = await _carRepo.GetByIdAsync(item.CarId);
                if (item.Car == null)
                {
                    TempData["ErrorMessage"] = $"Bilobjektet med ID: {item.CarId} kunde inte hittas.";
                    return View("ErrorPage", "Home");
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
            var car = await _carRepo.GetByIdAsync(carId);
            if (carId <= 0 || car == null)
            {
                TempData["ErrorMessage"] = $"Bilobjekt med ID: {carId} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }
            return View(car);
        }

        // POST: AdminController/EditCar/5,Car
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditCar(Car car)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _carRepo.UpdateAsync(car);
                    return RedirectToAction(nameof(ListAllAvailableCars));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Bilobjektet kunde inte uppdateras i databasen.");
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

        // GET: AdminController/ListAllAvailableCars
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

                        // Set User [ForeignKey] AccountId to same as Account Id
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
            var user = await _userRepo.GetByIdAsync(id);
            if (id <= 0 || user == null)
            {
                TempData["ErrorMessage"] = $"Användare med ID: {id} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }           

            var account = await _accRepo.GetWithAdressAsync(user.AccountId);
            if (user.AccountId <= 0 || account == null)
            {
                TempData["ErrorMessage"] = $"Konto med ID: {id} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }
            else if(account.AdressId <= 0 || account.Adress == null)
            {
                TempData["ErrorMessage"] = $"Adress med ID: {id} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }
            else
            {
                var model = CreateDeleteVM(user, account, account.Adress);
                return View(model);
            } 
        }

        // POST: AdminController/DeleteConfirm/ model
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> DeleteConfirm(DeleteUserViewModel model)
        {
            if(model.User == null || model.Account == null || model.Adress == null)
            {
                TempData["ErrorMessage"] = $"Användare med ID {model.User.UserId} kunde inte raderas.";
                return View("ErrorPage", "Home");
            }
            else
            {
                List<Booking> accountBookings = new List<Booking>
                           (await _bookRepo.GetBookingsByAccountIdAsync(model.Account.AccountId));

                // Update Account bookings to apply GDPR
                UpdateBookings(accountBookings);

                await _userRepo.DeleteAsync(model.User);
                await _adrRepo.DeleteAsync(model.Adress);
                await _accRepo.DeleteAsync(model.Account);

                return RedirectToAction(nameof(ListAllAccounts));
            }
        }

        // GET: AdminController/EditAccount/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (id <= 0 || User == null)
            {
                TempData["ErrorMessage"] = $"Användare med ID {id} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }

            var account = await _accRepo.GetByIdAsync(user.AccountId);
            if (user.AccountId <= 0 || account == null)
            {
                TempData["ErrorMessage"] = $"Användarkonto med ID {user.AccountId} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }

            var adress = await _adrRepo.GetByIdAsync(account.AccountId);
            if (account.AdressId <= 0 || adress == null)
            {
                TempData["ErrorMessage"] = $"Adressen med ID {account.AdressId} kunde inte hittas.";
                return View("ErrorPage", "Home");
            }

            var modelVM = CreateEditVM(user, account, account.Adress);
            
            return View(modelVM);
        }

        // POST: AdminController/EditAccount/Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(EditAccountViewModel model)
        {
            var user = await _userRepo.GetByIdAsync(model.UserId);
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

        public bool IsBookingFinished(Booking booking)
        {
            if (booking.EndDate < DateOnly.FromDateTime(DateTime.Now) && booking.IsFinished == false)
            {
                booking.IsFinished = true;
                return true;
            }
            return false;
        }

        public AllBookingsViewModel CreateBookingVM(Booking booking)
        {
            var model = new AllBookingsViewModel
            {
                BookingId = booking.BookingId,
                AccountId = booking.AccountId,
                //Email = account.Email,
                CarId = booking.CarId,
                Car = booking.Car,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalCost = booking.TotalCost,
                IsFinished = booking.IsFinished
            };
            return model;
        }

        public AllBookingsViewModel CreateBookingVM(Booking booking, string email)
        {
            var model = new AllBookingsViewModel
            {
                BookingId = booking.BookingId,
                AccountId = booking.AccountId,
                Email = email,
                CarId = booking.CarId,
                Car = booking.Car,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalCost = booking.TotalCost,
                IsFinished = booking.IsFinished
            };
            return model;
        }

        public EditAccountViewModel CreateEditVM(User user, Account account, Adress adress)
        {
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
            return model;
        }

        public string IsAccountGDPR(Account account, AllBookingsViewModel objekt)
        {

            if (account == null || string.IsNullOrEmpty(account.Email))
            {
                objekt.Email = "--GDPR--";
            }
            else
            {
                objekt.Email = account.Email;
            }

            return objekt.Email;
        }

        public DeleteUserViewModel CreateDeleteVM(User user, Account account, Adress adress)
        {
            var model = new DeleteUserViewModel
            {
                User = user,
                Account = account,
                Adress = account.Adress
            };
            return model;
        }

        public async Task UpdateBookings(List<Booking> accountBookings)
        {
            foreach (var booking in accountBookings)
            {
                // If a booking is not finished, erase it
                if (!booking.IsFinished)
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
        }

        #endregion
    }
}
