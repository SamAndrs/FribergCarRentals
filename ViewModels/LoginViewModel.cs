using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FribergRentalCars.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string EmailOrUserName { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte!")]
        public string ConfirmPassword { get; set; } = "";        
    }
}
