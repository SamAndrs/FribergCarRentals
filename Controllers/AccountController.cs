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

        // GET: AccountController/ChangeEmail
        public async Task<ActionResult> ChangeEmail(int id)
        {
            ViewBag.UserName = HttpContext.Session.GetString("user");

            var account = await _accountRepo.GetByIdAsync(id);
            if (!IsObjectValid(id, account, $"Användare kunde inte hittas."))
                return View("ErrorPage", "Home");           

            EmailViewModel emailVM = new EmailViewModel
            {
                Account = account,
                OldEmail = account.Email
            };
            return View(emailVM);
        }

        // POST: AccountController/ChangePassword/model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(EmailViewModel emailVM)
        {
            if (await _accountRepo.EmailAvailability(emailVM.NewEmail))
            {
                ModelState.AddModelError("", "Den här emailadressen är redan registrerad.");
                return View(emailVM);
            }
            else if (emailVM.NewEmail != emailVM.ConfirmNewEmail)
            {
                ModelState.AddModelError("", "Email adresserna matchar inte!");
                return View(emailVM);
            }
            else
            {
                var account = await _accountRepo.GetByIdAsync(emailVM.Account.AccountId);
                account.Email = emailVM.NewEmail;
                await _accountRepo.UpdateAsync(account);
                return RedirectToAction(nameof(Details));
            }
        }


        // GET: AccountController/ChangePassword
        public async Task<ActionResult> ChangePassword()
        {
            var userName = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(userName))
            {
                return NotFound(userName);
            }
            ViewBag.UserName = userName;

            var user = await _userRepo.GetUserByUserNameAsync(userName);
            if (!IsObjectValid(user.UserId, user, $"Användare kunde inte hittas."))
                return View("ErrorPage", "Home");

            PasswordViewModel model = new PasswordViewModel
            {
                User = user
            };
            return View(model);
        }

        // POST: AccountController/ChangePassword/model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(PasswordViewModel model)
        {
            var user = await _userRepo.GetByIdAsync(model.User.UserId);
            if (model.OldPassword != user.Password)
            {
                ModelState.AddModelError("", "Fel lösenord!");
                return View(model);
            }
            else
            {
                try
                {
                    user.Password = model.NewPassword;
                    user.ConfirmPassword = model.NewPassword;
                    await _userRepo.UpdateAsync(user);
                    return RedirectToAction(nameof(Details));
                }
                catch
                {
                    ModelState.AddModelError("", "Det gick inte att ändra lösenordet!");
                }
            }
            return View(model);
        }

        // GET: AccountController/Details/5
        public async Task<IActionResult> Details()
        {
            var account = await _accountRepo.GetWithAdressAsync((int)HttpContext.Session.GetInt32("accountID")!);
            
            var userName = HttpContext.Session.GetString("user");
            
            ViewBag.UserName = userName;
            
            return View(account);           
        }


        // GET: AccountController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userName = HttpContext.Session.GetString("user");
            ViewBag.UserName = userName;

            var account = await _accountRepo.GetWithAdressAsync(id);
            if (!IsObjectValid(id, account, $"Användarkonto kunde inte hittas."))
                return View("ErrorPage", "Home");

            return View(account);
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _accountRepo.UpdateAsync(account);
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Kontouppgifter gick inte att uppdatera.");
                }
                return RedirectToAction(nameof(Details));
            }
            return View(account);
        }


        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginVM)
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
                    HttpContext.Session.SetInt32("UserID", user.UserId);
                    return RedirectToAction("Details");
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

        // POST: AccountController/LoginRegister/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel regVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await _accountRepo.EmailAvailability(regVM.Account.Email))
                    {
                        ModelState.AddModelError("", "Den här emailadressen är redan registrerad.");
                        return View(regVM);
                    }
                    if (await _userRepo.UserNameAvailaibility(regVM.User.UserName))
                    {
                        ModelState.AddModelError("", "Det här användarnamnet är redan registrerat.");
                        return View(regVM);
                    }

                    await _accountRepo.AddAsync(regVM.Account);

                    regVM.User.AccountId = regVM.Account.AccountId;
                    await _userRepo.AddAsync(regVM.User);

                    HttpContext.Session.SetString("user", regVM.User.UserName);
                    HttpContext.Session.SetInt32("accountID", regVM.Account.AccountId);
                    HttpContext.Session.SetInt32("UserID", regVM.User.UserId);
                    return RedirectToAction(nameof(Details));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Det gick inte att skapa kontot: {ex.Message}");

                }
            }
            return View("LoginRegister", regVM);
        }

        // GET: AccountController/LoginRegister
        public ActionResult LoginRegister()
        {
            var model = new LoginRegisterViewModel
            {
                LoginVM = new LoginViewModel(),
                RegVM = new RegisterViewModel()

            };
            return View(model);
        }


        #region // HELPER METHODS ------------------------------------------------------

        public bool IsObjectValid(int id, Object obj, string errormessage)
        {
            if (id <= 0 || obj == null)
            {
                TempData["ErrorMessage"] = errormessage;
                Console.WriteLine($"Objekt med ID: {id} ej hittat");
                return false;
            }
            return true;
        }

        public bool IsObjectValid(int id, string errormessage)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = errormessage;
                Console.WriteLine($"Objekt med ID: {id} ej hittat");
                return false;
            }
            return true;
        }
        #endregion
    }
}

   
    
