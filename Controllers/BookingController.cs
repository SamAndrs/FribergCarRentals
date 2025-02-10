using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

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

            bool hasBookings = bookings.Any();
            if(hasBookings)
            {
                foreach (var item in bookings)
                {
                    item.Car = await _carRepo.GetByIdAsync(item.CarId);
                    if (!IsObjectValid(item.CarId, item.Car, $"Bil objekt kunde inte hittas."))
                        return View("ErrorPage", "Home");

                    if (CheckFinished(item))
                    {
                        await _bookRepo.UpdateAsync(item);
                    }
                }
            }
            else
                ViewBag.EmptyList = "Det finns inga registrerade bokningar.";

            
            return View(bookings);
        }


        // POST: BookingController/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(int id)
        {
            var booking = await _bookRepo.GetByIdAsync(id);
            if (!IsObjectValid(id, booking, $"Bokningen kunde inte hittas."))
                return View("CancelResult", "Booking");

            else
            {
                await _bookRepo.DeleteAsync(booking);
                TempData["SuccessMessage"] = "Avbokning lyckad!";
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
            if (!IsObjectValid(id, car, $"Bilen kunde inte hittas."))
                return View("ErrorPage", "Home");

            var bookVM = CreateBookingVM(car, (int)HttpContext.Session.GetInt32("accountID")!);
            
            return View(bookVM);
        }

        // POST: BookingController/Create/model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookingViewModel bookVM)
        {
            ViewBag.UserName = HttpContext.Session.GetString("user");

            var car = await _carRepo.GetByIdAsync(bookVM.CarId);
            if (!IsObjectValid(bookVM.CarId, car, $"Bilen kunde inte hittas."))
                return View("ErrorPage", "Home");

            var newBooking = CreateNewBooking(bookVM, car);

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
                        TempData["SuccessMessage"] = "Tack för din bokning!";
                        return View("Confirmation", newBooking);
                    }
                } 
            }
            TempData["ErrorMessage"] = "Bokningen kunde inte genomföras!";
            return View(bookVM);
        }

        // GET: BookingController/ListAccountBookings/5
        public async Task<ActionResult> FinishedAccountBookings(int id)
        {
            var bookings = await _bookRepo.GetFinishedAccountBookings(id);
            if (!IsObjectValid(id, $"Felaktig bokning."))
                return View("ErrorPage", "Home");

            ViewBag.AccountId = HttpContext.Session.GetInt32("accountID");
           
            foreach (var item in bookings)
            {
                item.Car = await _carRepo.GetByIdAsync(item.CarId);
                if (!IsObjectValid(item.CarId, item.Car, $"En bil kunde inte hittas."))
                    return View("ErrorPage", "Home");
                                
                if(CheckFinished(item))
                {
                    await _bookRepo.UpdateAsync(item);
                }
            }
            return View(bookings);
        }

        #region // HELPER METHODS ---------------------------------------------------
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

        public Booking CreateNewBooking(BookingViewModel bookVM, Car car)
        {
            Booking newBooking = new Booking
            {
                AccountId = bookVM.AccountId,
                CarId = bookVM.CarId,
                StartDate = bookVM.StartDate,
                EndDate = bookVM.EndDate,
                TotalCost = bookVM.TotalCost,
                Car = car,
                IsFinished = bookVM.IsFinished,
            };

            return newBooking;
        }
        
        public BookingViewModel CreateBookingVM(Car car, int id)
        {
           var bookVM = new BookingViewModel
            {
                CarId = car.CarId,
                AccountId = id,
                TotalCost = car.PricePerDay,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };

            return bookVM;
        }

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
