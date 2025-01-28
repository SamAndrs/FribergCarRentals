using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using FribergRentalCars.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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

        #endregion


        #region // CARS

        #endregion


        #region // ACCOUNTS

        // GET: AdminController/CreateAccount
        [AdminAuthorizationFilter]
        public ActionResult CreateAccount()  // TO DO
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAccount(RegisterViewModel regVM)  // TO DO
        {
            return View();
        }


        // GET: AdminController/DeleteAccount/5  // TO DO
        [AdminAuthorizationFilter]
        public ActionResult DeleteAccount(int id)
        {
            return View();
        }

        // POST: AdminController/DeleteAccount/5  // TO DO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
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
                    }
                    else
                    {
                        user.UserName = model.UserName;
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

            //return RedirectToAction(nameof(ListAllAccounts));

            return View(model);
        }

        #region OLD_EDIT 
        // GET: AdminController/ EditUser/5
        /*
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditUser(int userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }

        
        // POST: AdminController/EditUser/5, user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(int id, User user)
        {
            if(id != user.UserId)
            {
                NotFound();
            }
            
            if (ModelState.IsValid)
            {
                ViewBag.UserName = user.UserName;
                try
                {
                    await _userRepo.UpdateAsync(user);
                    
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(user.UserId == null)
                    {
                        return NotFound(user.UserId);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ListAllAccounts");
            }

            return View(user);
        }


        // GET: AdminController/ EditAccount/5
        [AdminAuthorizationFilter]
        public async Task<ActionResult> EditAccount(int accountId)
        {
            if (accountId == null)
            {
                return NotFound();
            }
            //var account = await _accRepo.GetIdByAsync(accountId);
            var account = await _accRepo.GetWithAdressAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: AdminController/EditAccount/5, account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                NotFound();
            }
            //var adress = await _adrRepo.GetIdByAsync(account.AdressId);
            //account.Adress.Street = adress.Street;
            if (ModelState.IsValid)
            {
                try
                {
                    await _accRepo.UpdateAsync(account);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (account.AccountId == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details));
            }
            return View(account);
        }*/
        #endregion

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

    }
}
