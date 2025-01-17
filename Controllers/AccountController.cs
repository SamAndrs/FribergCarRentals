using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Text.Json;

namespace FribergRentalCars.Controllers
{
    public class AccountController : Controller
    {
        public readonly ICustomerRepository _customRepo;

        private readonly IUserRepository _userRepo;

        public AccountController(ICustomerRepository customerRepository, IUserRepository userRepository)
        {
            this._customRepo = customerRepository;

            this._userRepo = userRepository;
        }

        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AccountController/Details/5
        public async Task <IActionResult> Details(int id)
        {
            var user = await _userRepo.GetUserByUserNameAsync(HttpContext.Session.GetString("user"));

            var customer = await _customRepo.GetWithAdressAsync(user.CustomerId);
            if (customer == null)
            {
                NotFound();
            }
            return View(customer);
        }


        // GET: AccountController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _customRepo.GetWithAdressAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _customRepo.EmailAvailability(customer.Email))
                {
                    ModelState.AddModelError("", "Den här emailadressen är redan registrerad.");
                }
                else
                {
                    try
                    {
                        await _customRepo.UpdateAsync(customer);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (customer.CustomerId == null)
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
            }
            return View(customer);
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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


        public ActionResult LoginSuccess()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

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
                    

                    if (user.IsAdmin)
                    {
                        //Session["UserId"] == userName.UserId;
                        // Session setup
                        /*
                        string json = JsonSerializer.Serialize(user);
                        HttpContext.Session.SetString("sessionUser", json);
                        */
                        HttpContext.Session.SetString("user", user.UserName);
                        return RedirectToAction("Index", "Home"); // TO-DO Admin page  and controller redirect
                    }
                    else
                    {
                        /*
                        string json = JsonSerializer.Serialize(user);
                        HttpContext.Session.SetString("sessionUser", json);
                        */
                        HttpContext.Session.SetString("user", user.UserName);
                        return RedirectToAction("Details");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Lösenordet eller användarnamn/ email är inkorrekt.");
                }
            }
            return View(loginVM);
        }
        
        // POST: AccountController/ Logout
        public ActionResult Logout()
        {
            return View();
        }


        // POST: AccountController/RegisterSuccess
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        // POST: AccountController/Register
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
                    if(await _customRepo.EmailAvailability(registerVM.Customer.Email))
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
                            await _customRepo.AddAsync(registerVM.Customer);

                            registerVM.User.CustomerId = registerVM.Customer.CustomerId;
                            await _userRepo.AddAsync(registerVM.User);

                            string json = JsonSerializer.Serialize(registerVM.User);
                            HttpContext.Session.SetString("sessionUser", json);
                            return RedirectToAction("RegisterSuccess"); // TO DO: Fixa register success till Account details page instead
                                                                        //return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                }
            }
            return View(registerVM);
        }

    }
}
