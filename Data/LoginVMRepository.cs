﻿using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using FribergRentalCars.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class LoginVMRepository : ILoginRepository
    {
        private readonly ApplicationDBContext _appDbContext;

        public LoginVMRepository(ApplicationDBContext applicationDBContext)
        {
            this._appDbContext = applicationDBContext;
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
           var customer = _appDbContext.Customers.FirstOrDefault(c => c.Email == email);
            return customer;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _appDbContext.Customers.FindAsync(id);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _appDbContext.Users.FindAsync(id);
        }
    }
}
