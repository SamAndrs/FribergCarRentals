using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using FribergRentalCars.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergRentalCars.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAccountRepository _accRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICarRepository _carRepo;
        private readonly IBookingRepository _bookRepo;

        public AdminController(IAccountRepository accRepo, IUserRepository userRepo, ICarRepository carRepo, IBookingRepository bookRepo)
        {
            this._accRepo = accRepo;
            this._userRepo = userRepo;
            this._carRepo = carRepo;
            this._bookRepo = bookRepo;
        }

        #region // ADMIN


        // GET: AdminController/Details/5
        [AdminAuthorizationFilter]
        public ActionResult Details(int id) // TO DO: VIew
        {
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

        // GET: AdminController/ListAllAccounts
        [AdminAuthorizationFilter]
        public async Task<ActionResult> ListAllAccounts()
        {
            var allUsers = await _userRepo.GetAllAsync();
            List<AllAccountsViewModel> accVMList = new List<AllAccountsViewModel>();

            foreach(var user in allUsers)
            {
                var account = await _accRepo.GetIdByAsync(user.AccountId);
                var newAccVM = new AllAccountsViewModel
                {
                    UserId= user.UserId,
                    AccountId= account.AccountId,
                    User = user,
                    Account = account,
                };
                accVMList.Add(newAccVM);
            }

            return View(accVMList);
        }

        #endregion

    }
}
