using FribergRentalCars.Models;

namespace FribergRentalCars.ViewModels.AdminViewModels
{
    public class AllBookingsViewModel
    {
        public int BookingId { get; set; }

        public int? AccountId { get; set; }

        public string Email { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int TotalCost { get; set; }

        

    }
}
