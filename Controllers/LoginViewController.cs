using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FribergRentalCars.ViewModels;
using FribergRentalCars.Models;
using FribergRentalCars.Data.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace FribergRentalCars.Controllers
{

    public class LoginViewController : Controller
    {
        private readonly ILoginRepository _repository;

        public LoginViewController(ILoginRepository loginRepository)
        {
            this._repository = loginRepository;
        }

        public ActionResult Success()
        {
            return View();
        }

        public async Task<ActionResult> Login(LoginViewModel loginVM)
        {
            if(ModelState.IsValid)
            {
                var userName = await _repository.GetUserByUserNameAsync(loginVM.EmailOrUserName);

                if(userName == null)
                {
                    try
                    {
                        userName = await _repository.GetUserByEmailAsync(loginVM.EmailOrUserName);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Användaren finns inte.");
                    }
                }

                if (loginVM.Password == userName.Password)
                {
                    //Session["UserId"] == userName.UserId;
                    // Session setup
                    string json = JsonSerializer.Serialize(userName);
                    HttpContext.Session.SetString("sessionUser", json);


                    return RedirectToAction("Success");
                }
                else
                {
                    ModelState.AddModelError("", "Lösenordet eller användarnamn/ email är inkorrekt.");
                }
            }
            return View(loginVM);
        }
                
       
    }
}
