using FribergRentalCars.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.ViewModels
{
    public class PasswordViewModel
    {
        public User User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8)]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Båda lösenord måste stämma!")]
        public string ConfirmNewPassword { get; set; }
    }
}
