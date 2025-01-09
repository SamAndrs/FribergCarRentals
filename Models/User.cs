using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public bool IsAdmin { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }
    }
}
