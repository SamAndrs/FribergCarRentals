using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergRentalCars.Controllers
{
    public class RegisterViewController : Controller
    {
        private readonly IRegisterRepository _repository;

        public RegisterViewController(IRegisterRepository registerRepository)
        {
            this._repository = registerRepository;
        }
        
        public ActionResult Register()
        {
            return View();
        }

        /*
        public ActionResult Success()
        {
            return View();
        }*/

        // POST: RegisterViewController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerVM)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _repository.AddAsync(registerVM);
                    return RedirectToAction("Success", "LoginView");
                    //return RedirectToAction("Index", "Home");
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
