using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface ICarRepository : IRepository<Car>
    {
        Task<IEnumerable<Car>> GetAllAvailableAsync();

        Task<IEnumerable<Car>> GetAllUnAvailableAsync();
    }
}
