using System.Diagnostics;
using System.Text.Json;
using FribergRentalCars.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergRentalCars.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Read from session variable
            /*
            int? sessionId = HttpContext.Session.GetInt32("sessionId");
            if(sessionId == null)
            {
                sessionId = 1;
            }
            else
            {
                sessionId += 1;
            }
            HttpContext.Session.SetInt32("sessionId", (int)sessionId!);
            */
                return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
