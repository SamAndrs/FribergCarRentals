using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.ViewModels
{
    public class EmailViewModel
    {
        public Account Account { get; set; }

        public string  OldEmail { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        public string? ConfirmNewEmail { get; set; }
    }
}
