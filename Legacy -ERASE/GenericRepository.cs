
using FribergRentalCars.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _appDbContext;
        
        public GenericRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _appDbContext.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetIdByAsync(id);
            if(entity != null)
            {
                _appDbContext.Set<T>().Remove(entity);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _appDbContext.Set<T>().ToListAsync();        
        }

        public async Task<T> GetIdByAsync(int id)
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _appDbContext.Update(entity);
            await _appDbContext.SaveChangesAsync();
        }

        Task<IEnumerable<T>> IRepository<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
