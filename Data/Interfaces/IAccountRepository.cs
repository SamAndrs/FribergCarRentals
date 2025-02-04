using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();

        Task<Account> GetIdByAsync(int id);

        Task<Account> GetWithAdressAsync(int id);

        Task AddAsync(Account account);

        Task UpdateAsync(Account account);

        Task DeleteAsync(Account account);

        Task<bool> EmailAvailability(string email);
    }
}
