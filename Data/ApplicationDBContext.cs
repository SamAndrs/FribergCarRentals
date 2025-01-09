using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars.Data
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Adress> Adresses { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
    }
}
