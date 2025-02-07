using FribergRentalCars.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergRentalCars.ViewModels.AdminViewModels
{
    public class EditAccountViewModel
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int AdressId { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password  { get; set; }

        public bool IsAdmin { get; set; }
    }
}
