using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;

namespace FribergRentalCars.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public BookingRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public Task AddAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Booking>> GetAllAsync()
        {
            throw new NotImplementedException();
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
