using System.ComponentModel;
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

        public Customer Customer { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; } = "";
    }
}
