using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FribergRentalCars.ViewModels;
using FribergRentalCars.Models;
using FribergRentalCars.Data.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FribergRentalCars.Controllers
{

    public class LoginViewController : Controller
    {
        private readonly ILoginRepository _repository;

        public LoginViewController(ILoginRepository loginRepository)
        {
            this._repository = loginRepository;
        }

        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if(ModelState.IsValid)
            {
                var user = await _repository.GetUserByIdAsync(loginVM.Customer.CustomerId);
                var email = await _repository.GetCustomerByEmail(loginVM.Customer.Email);
                
                if(user != null)
                {
                    if(user.UserName == loginVM.User.UserName || user.Customer.Email == loginVM.Customer.Email)
                    {
                        if(user.Password == loginVM.User.Password)
                        {
                            return RedirectToAction("Success", "LoginView");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Lösenordet är inkorrekt.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Användarnamnet eller Email adressen är inkorrekt.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Användaren finns inte.");
                }
            }
            return View(loginVM);
        }
                
        public ActionResult Success()
        {
            return View();
        }
    }
}
