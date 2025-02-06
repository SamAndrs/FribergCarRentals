using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public AccountRepository(ApplicationDBContext applicationDbContext)
        {
            this._appDbContext = applicationDbContext;
        }

        
        public async Task AddAsync(Account account)
        {
            await _appDbContext.Adresses.AddAsync(account.Adress);
            await _appDbContext.SaveChangesAsync();

            account.AdressId = account.Adress.AdressId;

            await _appDbContext.Accounts.AddAsync(account);
            await _appDbContext.SaveChangesAsync();
        }
       
        public async Task DeleteAsync(Account account)
        {
            _appDbContext.Accounts.Remove(account);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _appDbContext.Accounts.Include(a => a.Adress).ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _appDbContext.Accounts.FindAsync(id);
        }

        public async Task<Account> GetWithAdressAsync(int id)
        {
            return await _appDbContext.Accounts
                .Include(a => a.Adress)
                .FirstOrDefaultAsync(a=>a.AccountId == id);
        }

        public async Task UpdateAsync(Account account)
        {
            _appDbContext.Update(account);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> EmailAvailability(string email)
        {
            var isTaken = await _appDbContext.Accounts.AnyAsync(a => a.Email == email);
            return isTaken;
        }
            
    }
}
