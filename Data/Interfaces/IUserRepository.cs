using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetIdByAsync(int id);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);

        Task<User> GetUserByUserNameAsync(string userName);

        Task<User> GetUserByEmailAsync(string email);

        Task<bool> UserNameAvailaibility(string userName);
    }
}
