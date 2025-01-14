using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public UserRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }
        /*
        public async Task AddAsync(User user)
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }
        */
        public async Task DeleteAsync(User user)
        {
            _appDbContext.Remove(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<User> GetIdByAsync(int id)
        {
            return await _appDbContext.Users.FindAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

    }
}
