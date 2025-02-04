using FribergRentalCars.Models;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IAdressRepository
    {
        Task<IEnumerable<Adress>> GetAllAsync();

        Task<Adress> GetIdByAsync(int id);

        Task AddAsync(Adress adress);

        Task UpdateAsync(Adress adress);

        Task DeleteAsync(Adress adress);
    }
}
