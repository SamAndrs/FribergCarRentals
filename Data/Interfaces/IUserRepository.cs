using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
       Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByUserNameAsync(string userName);

        Task<string> FindUserNameByIdAsync(int id);

        Task<bool> UserNameAvailaibility(string userName);
    }
}
