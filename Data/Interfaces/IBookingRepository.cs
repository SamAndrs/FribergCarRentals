using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetAllActiveAsync();

        Task<IEnumerable<Booking>> GetAllFinishedAsync();

        Task<IEnumerable<Booking>> GetBookingsByCarIdAsync(int id);

        Task<IEnumerable<Booking>> GetBookingsByAccountIdAsync(int id);

        Task<IEnumerable<Booking>> GetActiveAccountBookings(int id);

        Task<IEnumerable<Booking>> GetFinishedAccountBookings(int id);
    }
}
