
using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public CarRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task AddAsync(Car car)
        {
            await _appDbContext.Cars.AddAsync(car);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Car car)
        {
            /*_appDbContext.Set<Car>().Remove(car);*/
            _appDbContext.Cars.Remove(car);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _appDbContext.Cars.ToListAsync();//  Set<Car>().ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetAllAvailable()
        {
            return await _appDbContext.Cars.Where(c => c.IsAvailable).ToListAsync();

            /*return await _appDbContext.Cars.Where(c => c.IsAvailable).ToListAsync();*/
        }

        public async Task<Car> GetIdByAsync(int id)
        {
            return await _appDbContext.Cars.FindAsync(id);
            /*return await _appDbContext.Set<Car>().FirstOrDefaultAsync(c => c.CarId == id);*/
        }

        public async Task UpdateAsync(Car car)
        {
            _appDbContext.Update(car);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
