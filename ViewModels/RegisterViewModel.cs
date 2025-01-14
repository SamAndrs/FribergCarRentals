using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.ViewModels
{
    public class RegisterViewModel
    {
        public Customer Customer { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = "Must confirm password.")]
        [DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
