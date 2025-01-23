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
        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();
            return View(cars);
        }

        // GET: CarController/GetAvailable
        public async Task<IActionResult> GetAvailable()
        {
            var availableCars = await _carRepository.GetAllAvailable();
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

        // GET: CarController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Car car)
        {
            try
            {
                if(ModelState.IsValid)
                {
                  _carRepository.AddAsync(car);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CarController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var car = await _carRepository.GetIdByAsync(id);

            if(car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
           if( id != car.CarId)
            {
                return NotFound();
            }

           if(ModelState.IsValid)
            {
                try
                {
                    _carRepository.UpdateAsync(car);
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(car.CarId == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }    

        // GET: CarController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Car car)
        {
            try
            {
                _carRepository.DeleteAsync(car);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
