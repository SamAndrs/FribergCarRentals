using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IUserRepository 
    {
        Task AddAsync(User user);

        Task DeleteAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(int id);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByUserNameAsync(string userName);

        Task<string> FindUserNameByIdAsync(int id);

        Task<bool> UserNameAvailaibility(string userName);

        Task UpdateAsync(User user);

        

        

       

        
    }
}
