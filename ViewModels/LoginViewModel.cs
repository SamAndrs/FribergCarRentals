using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FribergRentalCars.ViewModels
{
    public class LoginViewModel
    {
        public Customer Customer { get; set; }

        public User User { get; set; }
        
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Must confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match!")]
        public string ConfirmPassword { get; set; } = "";
        
        [DisplayName(displayName: "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
