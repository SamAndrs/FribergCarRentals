using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;

namespace FribergRentalCars.Data.Interfaces
{
    public interface ILoginRepository
    {
        Task<Customer> GetCustomerByIdAsync(int id);

        Task<User> GetUserByIdAsync(int id);

        Task<Customer> GetCustomerByEmail(string email);

    }
}
