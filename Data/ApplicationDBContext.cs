using FribergRentalCars.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FribergRentalCars.Data
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Adress> Adresses { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<User> Users { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }

        //public DbSet<FribergRentalCars.Models.Admin> Admin { get; set; } = default!;

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Account)
                .WithMany()
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
            
        }*/
        /*
         modelBuilder.Entity<Booking>()
    .HasOne(b => b.Account)
    .WithMany(a => a.Bookings)
    .HasForeignKey(b => b.AccountId)
    .OnDelete(DeleteBehavior.Restrict);
         */
    }
}
