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
            var car = await _carRepository.GetIdByAsync(id);
            if(car == null)
            {
                return NotFound();
            }
            return View(car);
        }
    }
}
