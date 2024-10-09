using Microsoft.AspNetCore.Mvc;
using TimeOffManager.Models;
using System.Diagnostics;
using TimeOffManager.Data;

namespace TimeOffManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly URDbContext _db;


        public HomeController(URDbContext dbContext)
        {
            _db = dbContext;
        }

        public IActionResult Index()
        {
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
