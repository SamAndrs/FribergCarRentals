using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FribergRentalCars.Controllers
{
    public class BookingController : Controller
    {
        private readonly ICarRepository _carRepo;
        private readonly ICustomerRepository _custRepo;
        private readonly IBookingRepository _bookRepo;

        public BookingController(ICarRepository carRepository, ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            this._carRepo = carRepository;
            this._custRepo = customerRepository;
            this._bookRepo = bookingRepository;
        }
        // GET: BookingController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BookingController
        public async Task<ActionResult> ListAll()
        {
            var bookings = await _bookRepo.GetAllAsync();
            return View(bookings);
        }


        // GET: BookingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> Confirmation(Booking booking)
        {
            //var booking = await _bookRepo.GetIdByAsync(id);
            var car = await _carRepo.GetIdByAsync(booking.CarId);
            return View(booking);
        }


        // GET: BookingController/Create
        public async Task<ActionResult> Create()
        {
            var carID = HttpContext.Session.GetInt32("carID");
            var car = await _carRepo.GetIdByAsync((int)carID!);
            BookingViewModel bookVM = new BookingViewModel
            {
                CarId = car.CarId,
                TotalCost = car.PricePerDay, // TO DO: räkna ut kostnaden för totala antalet dagar
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(bookVM);
        }

        // POST: BookingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmBooking(BookingViewModel bookVM)
        {
            Booking newBooking = new Booking
            {
                CustomerId = (int)HttpContext.Session.GetInt32("customerID")!,
                CarId = bookVM.CarId,
                TotalCost = bookVM.TotalCost,
                StartDate = bookVM.StartDate,
                EndDate = bookVM.EndDate,
                Car = await _carRepo.GetIdByAsync(bookVM.CarId)
            };     
            
            if (ModelState.IsValid)
            {
                if(bookVM.CarId == null)
                {
                    ModelState.AddModelError("", "Bilen finns inte!");
                }
                //else if(CustomerId == null)
                else if(newBooking.CustomerId == null)
                {
                    ModelState.AddModelError("", "Användaren finns inte!");
                }
                else if(bookVM.StartDate > bookVM.EndDate)
                {
                    ModelState.AddModelError("", "Slutdatum kan inte ligga före Startdatum!");
                    return View("Create", bookVM);
                }
                else
                { 
                    await _bookRepo.AddAsync(newBooking);
                    return View("Confirmation", newBooking);
                }
            }
            return View(bookVM);



            /* CHATGPT
              if (ModelState.IsValid)
    {
        var existingBooking = _context.Bookings
            .Where(b => b.CarId == model.CarId &&
                        (b.StartDate < model.EndDate && b.EndDate > model.StartDate))
            .FirstOrDefault();

        if (existingBooking != null)
        {
            ModelState.AddModelError("", "Den valda bilen är redan bokad under den perioden.");
            return View(model);
        }

        var booking = new Booking
        {
            CarId = model.CarId,
            CustomerId = model.CustomerId,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return RedirectToAction("Index", "Cars");
    }

    return View(model);
             * */
        }

        // GET: BookingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingController/Delete/5
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

       
    }
}
