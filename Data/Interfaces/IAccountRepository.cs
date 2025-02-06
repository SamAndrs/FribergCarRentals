using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetWithAdressAsync(int id);

        Task<bool> EmailAvailability(string email);
    }
}
