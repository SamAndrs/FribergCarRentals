using FribergRentalCars.Models;

namespace FribergRentalCars.Data
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();

        Task<Car> GetIdByAsync(int id);

        Task AddAsync(Car car);

        Task UpdateAsync(Car car);

        Task DeleteAsync(Car car);

        Task<IEnumerable<Car>> GetAllAvailable(); 
    }
}
