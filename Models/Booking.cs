using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("Car")]
        public int CarId { get; set; }
    }
}
