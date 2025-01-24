using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

namespace FribergRentalCars.Controllers
{
    public class AccountController : Controller
    {
        public readonly IAccountRepository _accountRepo;

        private readonly IUserRepository _userRepo;
        
        private readonly IBookingRepository _bookRepo;

        public AccountController(IAccountRepository accountRepository, IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            this._accountRepo = accountRepository;

            this._userRepo = userRepository;

            this._bookRepo = bookingRepository;
        }

        // GET: AccountController
        public ActionResult Index() // TO DO: CHANGE INTO ACCOUNT PAGE???
        {
            return View();
        }

        public ActionResult ChangePassword(PasswordVM passwordVM)
        {
            return View();
        }

        // GET: AccountController/Details/5
        public async Task <IActionResult> Details()
        {
            var account = await _accountRepo.GetWithAdressAsync((int)HttpContext.Session.GetInt32("accountID")!);
            if (account == null)
            {
                NotFound();
            }
            return View(account);
        }


        // GET: AccountController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var account = await _accountRepo.GetWithAdressAsync(id);

            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (id != account.AccountId)
            {
                NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _accountRepo.UpdateAsync(account);
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
        }

        // GET: AccountController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepo.GetUserByUserNameAsync(loginVM.EmailOrUserName);

                if (user == null)
                {
                    try
                    {
                        user = await _userRepo.GetUserByEmailAsync(loginVM.EmailOrUserName);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Användaren finns inte.");
                    }
                }

                if (loginVM.Password == user.Password)
                {
                    HttpContext.Session.SetInt32("accountID", user.AccountId);
                    HttpContext.Session.SetString("user", user.UserName);
                    return RedirectToAction("Details");
                    /*
                    if (user.IsAdmin)
                    {
                        // Using a numeric value for the 'isAdmin' value, since Session doesn't allow bools
                        HttpContext.Session.SetInt32("isAdmin", 1);
                        return RedirectToAction("Index", "Admin"); // TO-DO Admin page  and controller redirect
                    }
                    else
                    {
                        return RedirectToAction("Details");
                    }*/
                }
                else
                {
                    ModelState.AddModelError("", "Lösenordet eller användarnamn/ email är inkorrekt.");
                }
            }
            return View(loginVM);
        }
        
        // GET: AccountController/ Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }   

        // GET: AccountController/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: AccountController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(await _accountRepo.EmailAvailability(registerVM.Account.Email))
                    {
                        ModelState.AddModelError("", "Den här emailadressen är redan registrerad.");
                        return View(registerVM);
                    }
                    else
                    {
                       if(await _userRepo.UserNameAvailaibility(registerVM.User.UserName))
                        {
                            ModelState.AddModelError("", "Det här användarnamnet är redan registrerat.");
                            return View(registerVM);
                        }
                        else
                        {
                            await _accountRepo.AddAsync(registerVM.Account);

                            registerVM.User.AccountId = registerVM.Account.AccountId;
                            await _userRepo.AddAsync(registerVM.User);

                            HttpContext.Session.SetString("user", registerVM.User.UserName);
                            HttpContext.Session.SetInt32("accountID", registerVM.Account.AccountId);
                            return RedirectToAction(nameof(Details));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating account: {ex.Message}");
                }
            }
            return View(registerVM);
        }
    }
}
