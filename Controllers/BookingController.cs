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
        private readonly IAccountRepository _accRepo;
        private readonly IBookingRepository _bookRepo;

        public BookingController(ICarRepository carRepository, IAccountRepository accountRepository, IBookingRepository bookingRepository)
        {
            this._carRepo = carRepository;
            this._accRepo = accountRepository;
            this._bookRepo = bookingRepository;
        }

        // GET: BookingController/ListAccountBookings/5
        public async Task<ActionResult> ActiveAccountBookings(int id)
        {
            ViewBag.AccountId = HttpContext.Session.GetInt32("accountID");
            var bookings = await _bookRepo.GetActiveAccountBookings(id);
            if (id == null)
            {
                return NotFound(id);
            }

            foreach (var item in bookings)
            {
                try
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Bilen finns inte.");
                }

                if (CheckFinished(item))
                {
                    await _bookRepo.UpdateAsync(item);
                }
            }
            return View(bookings);
        }



        // POST: BookingController/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(int id)
        {
            try
            {
                var booking = await _bookRepo.GetByIdAsync(id);
                await _bookRepo.DeleteAsync(booking);
                TempData["SuccessMessage"] = "Avbokning lyckad!";
                return RedirectToAction(nameof(CancelResult));
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Avbokning misslyckades!";
                return RedirectToAction(nameof(CancelResult));
            }
        }

        // GET: BookingController/CancelConfirmation
        public ActionResult CancelResult()
        {
            return View();
        }

        public ActionResult Confirmation(Booking booking)
        {
            return View(booking);
        }

        // GET: BookingController/Create/5
        public async Task<ActionResult> Create(int id)
        {
            ViewBag.UserName = HttpContext.Session.GetString("user");
            var car = await _carRepo.GetByIdAsync(id);
            BookingViewModel bookVM = new BookingViewModel
            {
                CarId = car.CarId,
                AccountId = (int)HttpContext.Session.GetInt32("accountID")!,
                TotalCost = car.PricePerDay,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(bookVM);
        }

        // POST: BookingController/Create/model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookingViewModel bookVM)
        {
            ViewBag.UserName = HttpContext.Session.GetString("user");
            Booking newBooking = new Booking
            {
               AccountId = bookVM.AccountId,
               CarId = bookVM.CarId,
               StartDate = bookVM.StartDate,
               EndDate = bookVM.EndDate,
               TotalCost = bookVM.TotalCost,
               Car = await _carRepo.GetByIdAsync(bookVM.CarId),
               IsFinished = bookVM.IsFinished,
            };

            if (ModelState.IsValid)
            {
                if (bookVM.StartDate > bookVM.EndDate)
                {
                    ModelState.AddModelError("", "Slutdatum kan inte ligga före Startdatum!");
                }
                else
                {
                    // Check if car is already booked on dates 
                    var booked = await IsOverlapping(newBooking.CarId, newBooking);
                    if (booked)
                    {
                        ModelState.AddModelError("", "Bilen är redan bokad under dessa datum.");
                    }
                    else
                    {
                        newBooking.TotalCost = (newBooking.EndDate.DayNumber - newBooking.StartDate.DayNumber) * newBooking.Car.PricePerDay;
                        await _bookRepo.AddAsync(newBooking);
                        return View("Confirmation", newBooking);
                    }
                } 
            }
            return View(bookVM);
        }

        // GET: BookingController/ListAccountBookings/5
        public async Task<ActionResult> FinishedAccountBookings(int id)
        {
            ViewBag.AccountId = HttpContext.Session.GetInt32("accountID");
            var bookings = await _bookRepo.GetFinishedAccountBookings(id);
            if(id == null)
            {
                return NotFound(id);
            }
            
            foreach (var item in bookings)
            {
                try
                { 
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                }
                catch(Exception)
                {
                    ModelState.AddModelError("", "Bilen finns inte.");
                }
                
                if(CheckFinished(item))
                {
                    await _bookRepo.UpdateAsync(item);
                }
            }
            return View(bookings);
        }

        #region HELPER METHODS ---------------------------------------------------
        public async Task<bool> IsOverlapping(int carID, Booking booking)
        {
            var existingBookings = await _bookRepo.GetBookingsByCarIdAsync(carID);

            // Check if dates overlap with an existing booking that is not yet listed as 'finished'
            bool isOverlapping = existingBookings.Any(b =>
            (b.StartDate < booking.EndDate && b.EndDate > booking.StartDate && !b.IsFinished));
            return isOverlapping;
        }

        public bool CheckFinished(Booking booking)
        {
            if (booking.EndDate < DateOnly.FromDateTime(DateTime.Now) && booking.IsFinished == false)
            {
                booking.IsFinished = true;
                return true;
            }
            return false;
        }
        #endregion


    }
}
