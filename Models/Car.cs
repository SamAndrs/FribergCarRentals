namespace FribergRentalCars.Models
{
    public class Car
    {
        public int CarId { get; set; }

        public string Model { get; set; }

        public int ModelYear { get; set; }

        public List<string> CarImages { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }
    }
}
