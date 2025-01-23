namespace FribergRentalCars.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

    }
}
