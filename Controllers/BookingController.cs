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

        // GET: BookingController  TO DO: REMOVE (admin functionality)
        public async Task<ActionResult> ListAll()
        {
            var bookings = await _bookRepo.GetAllAsync();
            return View(bookings);
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
                TotalCost = car.PricePerDay,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(bookVM);
        }

        // POST: BookingController/ConfirmBooking
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
                    var booked = await IsOverlapping(newBooking.CarId, newBooking);
                    if (booked)
                    {
                        ModelState.AddModelError("", "Bilen är redan bokad under dessa datum.");
                        return View("Create", bookVM);
                    }

                    newBooking.TotalCost = (newBooking.EndDate.DayNumber - newBooking.StartDate.DayNumber) * newBooking.Car.PricePerDay;
                    await _bookRepo.AddAsync(newBooking);
                    //customer.Bookings.Add(newBooking);
                    return View("Confirmation", newBooking);
                }
            }
            return View(bookVM);
        }

        public async Task<ActionResult> ListAccountBookings(int id)
        {
            var bookings = await _bookRepo.GetBookingsByCustomerIdAsync(id);
            if(id == null)
            {
                return NotFound();
            }
            
            foreach (var item in bookings)
            {
                try
                {
                    item.Car = await _carRepo.GetIdByAsync(item.CarId);
                }
                catch(Exception)
                {
                    ModelState.AddModelError("", "Bilen är inte tillgänglig att hyra.");
                }
            }
            return View(bookings);
        }

        #region HELPER METHODS ---------------------------------------------------
        public async Task<bool> IsOverlapping(int carID, Booking booking)
        {
            var existingBookings = await _bookRepo.GetBookingsByCarIdAsync(carID);

            bool isOverlapping = existingBookings.Any(b =>
            (b.StartDate < booking.EndDate && b.EndDate > booking.StartDate));

            return isOverlapping;

        }
        #endregion


    }
}
