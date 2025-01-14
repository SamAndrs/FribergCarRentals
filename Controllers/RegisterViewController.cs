using FribergRentalCars.Data.Interfaces;
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

        // GET: RegisterViewController/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }

        // POST: RegisterViewController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel registerVM)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _repository.AddAsync(registerVM);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                }
            }
            return View();
        }
    }
}
