using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int CustomerId { get; set; }

        public int CarId { get; set; }
    }
}
