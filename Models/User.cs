using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ForeignKey("Adress")]
        public int AdressId { get; set; }

        public string PhoneNumber { get; set; }

        public string email { get; set; }
    }
}
