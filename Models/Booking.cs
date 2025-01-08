using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Car")]
        public int CarId { get; set; }
    }
}
