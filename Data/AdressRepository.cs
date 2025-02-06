using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class AdressRepository : IAdressRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public AdressRepository(ApplicationDBContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task AddAsync(Adress adress)
        {
            await _appDbContext.Adresses.AddAsync(adress);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Adress adress)
        {
            _appDbContext.Adresses.Remove(adress);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Adress>> GetAllAsync()
        {
            return await _appDbContext.Adresses.ToListAsync();
        }

        public async Task<Adress> GetByIdAsync(int id)
        {
            return await _appDbContext.Adresses.FindAsync(id);
        }

        public async Task UpdateAsync(Adress adress)
        {
            _appDbContext.Adresses.Update(adress);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
