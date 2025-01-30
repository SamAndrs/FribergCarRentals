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

        public async Task DeleteAsync(Booking booking)
        {
            _appDbContext.Bookings.Remove(booking);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _appDbContext.Bookings.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCarIdAsync(int id)
        {
            return await _appDbContext.Bookings.Where(b => b.CarId == id).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByAccountIdAsync(int id)
        {
            return await _appDbContext.Bookings.Where(b => b.AccountId == id).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetFinishedAccountBookings(int id)
        {
            return await _appDbContext.Bookings.
                Where(b => b.AccountId == id).
                Where(b => b.IsFinished == true).ToListAsync();
        }

        public async Task<Booking> GetIdByAsync(int id)
        {
            return await _appDbContext.Bookings.FindAsync(id);
        }

        public async Task UpdateAsync(Booking booking)
        {
            _appDbContext.Bookings.Update(booking);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
