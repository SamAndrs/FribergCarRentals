using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.ViewModels
{
    public class CarBookingVM
    {
        public int CustomerId { get; set; }

        public int CarId { get; set; }

        public int TotalCost { get; set; }

    }
}
