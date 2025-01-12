using FribergRentalCars.Models;

namespace FribergRentalCars.Data
{
    public interface ICustomer
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer> GetIdByAsync(int id);

        Task<Customer> GetWithAdressAsync(int id);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(Customer customer);
    }
}
