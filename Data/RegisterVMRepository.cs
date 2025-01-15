using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FribergRentalCars.Data
{
    public class RegisterVMRepository : IRegisterRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public RegisterVMRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task AddAsync(RegisterViewModel registerVM)
        {
            await _appDbContext.Customers.AddAsync(registerVM.Customer);
            await _appDbContext.SaveChangesAsync();
            
            registerVM.User.CustomerId = registerVM.Customer.CustomerId; 

            await _appDbContext.Users.AddAsync(registerVM.User);
            await _appDbContext.SaveChangesAsync();
           
        }
    }
}
