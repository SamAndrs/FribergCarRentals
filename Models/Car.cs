using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        public string Model { get; set; }

        public int ModelYear { get; set; }

        public virtual List<string> CarImages { get; set; }

        public int Price { get; set; }

        public string Description { get; set; }
    }
}
