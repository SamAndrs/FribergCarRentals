using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = "";
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(16, MinimumLength = 1)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords doesn't match!")]
        public string ConfirmPassword { get; set; }
    }
}
