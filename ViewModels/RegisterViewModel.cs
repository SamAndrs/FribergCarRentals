using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.ViewModels
{
    public class RegisterViewModel
    {
        public Account Account { get; set; }

        public User User { get; set; }


    }
}
