using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;

namespace FribergRentalCars.Data.Interfaces
{
    public interface IRegisterRepository
    {
        Task AddAsync(RegisterViewModel registerVM);
    }
}
