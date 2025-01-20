using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();

        Task<Booking> GetIdByAsync(int id);

        Task AddAsync(Booking booking);

        Task UpdateAsync(Booking booking);

        Task DeleteAsync(Booking booking);
    }
}
