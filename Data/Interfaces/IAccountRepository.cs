using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer> GetIdByAsync(int id);

        Task<Customer> GetWithAdressAsync(int id);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(Customer customer);

        Task<bool> EmailAvailability(string email);
    }
}
