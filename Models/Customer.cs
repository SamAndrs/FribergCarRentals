namespace FribergRentalCars.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public List<Booking> Bookings { get; set; }

    }
}
