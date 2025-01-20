using FribergRentalCars.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.ViewModels
{
    public class PasswordVM
    {
        public User User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8)]
        [DisplayName("Ange nytt lösenord")]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Båda lösenord måste stämma!")]
        [DisplayName("Bekräfta lösenord")]
        public string ConfirmNewPassword { get; set; }
    }
}
