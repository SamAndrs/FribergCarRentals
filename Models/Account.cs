using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Ett förnamn måste anges.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Ett efternamn måste anges.")]
        public string LastName { get; set; }

        public Adress? Adress { get; set; }

        [ForeignKey("Adress")]
        public int? AdressId { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "En emailadress måste anges.")]
        public string? Email { get; set; }

        public List<Booking>? Bookings { get; set; }

    }
}
