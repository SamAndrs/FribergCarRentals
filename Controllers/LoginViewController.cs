using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FribergRentalCars.ViewModels;
using FribergRentalCars.Models;
using FribergRentalCars.Data.Interfaces;

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
                var user = _repository.Authenticate
            }
            
        }
                
        public ActionResult Success()
        {
            return View();
        }
    }
}
