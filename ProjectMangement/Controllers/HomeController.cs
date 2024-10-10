using Microsoft.AspNetCore.Mvc;
using TimeOffManager.Models;
using System.Diagnostics;
using TimeOffManager.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(int UserId)
        {
            var user = await _db.Users.Include(u => u.Requests).FirstOrDefaultAsync(u => u.UserId == UserId);
            ViewBag.FirstName = user.FirstName; // Replace with actual user data
            ViewBag.LastName = user.LastName;
            return View(user);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("home/report/{RequestId}")]
        public async Task<IActionResult> Report(int RequestId)
        {
            if (RequestId != -1)
            {
                var report = await _db.Requests.Include(u => u.User).FirstOrDefaultAsync(r => r.RequestId == RequestId);
                if (report != null)
                {
                    ViewBag.FirstName = report.User.FirstName;
                    ViewBag.LastName = report.User.LastName;
                    return View(report);
                }
            }

            return View(new Request());
        }

        [HttpPost("home/report/{RequestId}")]
        public async Task<IActionResult> Report(Request request){
            if(request.RequestId == -1)
            {
                _db.Add(request);
            }
            else {  _db.Update(request);}
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
