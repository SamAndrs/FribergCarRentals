using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Adress? Adress { get; set; }

        [ForeignKey("Adress")]
        public int? AdressId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public List<Booking>? Bookings { get; set; }

    }
}
