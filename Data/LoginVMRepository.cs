using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class LoginVMRepository : ILoginRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public LoginVMRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
        
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var customer = await _appDbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.CustomerId == customer.CustomerId);
            return user;
        }
    }
}
