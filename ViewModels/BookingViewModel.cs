using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FribergRentalCars.ViewModels
{
    public class BookingViewModel
    {
        public int CarId { get; set; }

        public int TotalCost { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
        
        [Required]
        public DateOnly EndDate { get; set; }

        public bool IsFinished { get; set; }
    }
}
