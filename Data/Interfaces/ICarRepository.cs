using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface ICarRepository
    {
        Task AddAsync(Car car);

        Task DeleteAsync(Car car);

        Task<IEnumerable<Car>> GetAllAsync();

        Task<IEnumerable<Car>> GetAllAvailableAsync();

        Task<IEnumerable<Car>> GetAllUnAvailableAsync();

        Task<Car> GetIdByAsync(int id);

        Task UpdateAsync(Car car);
    }
}
