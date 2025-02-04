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
        
        public async Task AddAsync(User user)
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(User user)
        {
            _appDbContext.Remove(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _appDbContext.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var account = await _appDbContext.Accounts.FirstOrDefaultAsync(c => c.Email == email);
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.AccountId == account.AccountId);
            return user;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> UserNameAvailaibility(string userName)
        {
            var isTaken = await _appDbContext.Users.AnyAsync(c => c.UserName == userName);
            return isTaken;
        }

        public async Task<string> FindUserNameByIdAsync(int id)
        {
            var user = await _appDbContext.Users.Where(u => u.UserId == id).
                FirstOrDefaultAsync();

            if(user != null)
            {
                return user.UserName;
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

    }
}
