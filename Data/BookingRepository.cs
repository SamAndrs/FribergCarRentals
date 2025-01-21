using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public BookingRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task AddAsync(Booking booking)
        {
            await _appDbContext.Bookings.AddAsync(booking);
            await _appDbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _appDbContext.Bookings.ToListAsync();
        }

        public Task<Booking> GetIdByAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
