using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class CustomerRepository : ICustomer
    {
        private readonly ApplicationDBContext _appDbContext;

        public CustomerRepository(ApplicationDBContext applicationDbContext)
        {
            this._appDbContext = applicationDbContext;
        }

        public async Task AddAsync(Customer customer)
        {
            await _appDbContext.Adresses.AddAsync(customer.Adress);
            await _appDbContext.SaveChangesAsync();

            customer.AdressId = customer.Adress.AdressId;

            await _appDbContext.Customers.AddAsync(customer);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _appDbContext.Customers.Remove(customer);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _appDbContext.Customers.Include(c => c.Adress).ToListAsync();
        }

        public async Task<Customer> GetIdByAsync(int id)
        {
            return await _appDbContext.Customers.FindAsync(id);
        }

        public async Task<Customer> GetWithAdressAsync(int id)
        {
            return await _appDbContext.Customers
                .Include(c => c.Adress)
                .FirstOrDefaultAsync(c=>c.CustomerId == id);
        }

        public async Task UpdateAsync(Customer customer)
        {
            _appDbContext.Update(customer);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
