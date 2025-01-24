using FribergRentalCars.Models;

namespace FribergRentalCars.ViewModels.AdminViewModels
{
    public class AllAccountsViewModel
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public User? User { get; set; }

        public Account? Account { get; set; }
    }
}
