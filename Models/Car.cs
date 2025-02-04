using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        public string Model { get; set; }

        public int ModelYear { get; set; }

        public string? RegNumber { get; set; }

        //public virtual List<string> CarImages { get; set; }

        public string? Image { get; set; }

        public int PricePerDay { get; set; }

        public string? Description { get; set; }

        public bool IsAvailable { get; set; }
    }
}
