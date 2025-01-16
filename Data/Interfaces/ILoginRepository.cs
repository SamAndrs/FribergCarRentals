using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;

namespace FribergRentalCars.Data.Interfaces
{
    public interface ILoginRepository
    {
        Task<User> GetUserByUserNameAsync(string userName);

        Task<User> GetUserByEmailAsync(string email);

    }
}
