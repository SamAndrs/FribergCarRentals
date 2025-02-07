using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarRepository _carRepository;

        public CarController(ICarRepository carRepository)
        {
            this._carRepository = carRepository;
        }

        // GET: CarController
        public async Task<IActionResult> ListAll()
        {
            var cars = await _carRepository.GetAllAsync();
            return View(cars);
        }

        // GET: CarController/GetAvailable
        public async Task<IActionResult> GetAvailable()
        {
            var availableCars = await _carRepository.GetAllAvailableAsync();
            return View(availableCars);
        }

        // GET: CarController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (!IsObjectValid(id, car, $"Bilen kunde inte hittas."))
                return View("ErrorPage", "Home");

            return View(car);
        }


        #region // HELPER METHODS ---------------------------------------------------
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
