using FribergRentalCars.Data;
using Microsoft.EntityFrameworkCore;

namespace FribergRentalCars
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           //ar _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FribergCarsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDBContext>(
                options => options.UseSqlServer(new ConfigurationBuilder()
                                                .AddJsonFile("appsettings.json")
                                                .Build()
                                                .GetSection("ConnectionStrings")["FribergCarsDB"])
                );
            builder.Services.AddTransient<ICarRepository, CarRepository>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
