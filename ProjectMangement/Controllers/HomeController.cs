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
            UserId = (int)HttpContext.Session.GetInt32("UserId");
            var user = await _db.Users.Include(u => u.Requests).FirstOrDefaultAsync(u => u.UserId == UserId);
           
            int availablePTOHours = 160;  // Assuming you have these properties in the User model
            int availableFHHours = 8;
            int availableCIHours = 16;

            //Update hours
            foreach (var request in user.Requests)
            {
                if (request.Status.ToLower() == "approved") // Only consider approved requests
                {
                    // Calculate the total working days (excluding weekends)
                    int workingDays = CalculateWorkingDays(request.StartDate, request.EndDate);

                    // Each working day is 8 hours
                    int requestedHours = workingDays * 8;

                    // Deduct hours based on the request type
                    switch (request.RequestType)
                    {
                        case "PTO":
                            availablePTOHours -= requestedHours;
                            break;
                        case "FH":
                            availableFHHours -= requestedHours;
                            break;
                        case "CI":
                            availableCIHours -= requestedHours;
                            break;
                    }
                }
            }

            // Update the user's available hours in the database
            user.PTOHours = availablePTOHours;
            user.FHHours = availableFHHours;
            user.CIHours = availableCIHours;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return View(user);

        }

        public int CalculateWorkingDays(DateOnly startDate, DateOnly endDate)
        {
            int totalWorkingDays = 0;

            // Iterate from startDate to endDate
            for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Check if the day is not a weekend (Saturday or Sunday)
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalWorkingDays++;
                }
            }

            return totalWorkingDays;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("home/report/{RequestId}")]
        public async Task<IActionResult> Report(int RequestId, int UserId)
        {
            if (RequestId != 0)
            {
                var report = await _db.Requests.Include(u => u.User).FirstOrDefaultAsync(r => r.RequestId == RequestId);
                if (report != null)
                {
                    ViewBag.FirstName = report.User.FirstName;
                    ViewBag.LastName = report.User.LastName;
                    return View(report);
                }
            }

            return View(new Request { UserId = UserId});
        }

        [HttpDelete("home/delete")]
        public async Task<IActionResult> Delete(int RequestId, int UserId)
        {
            var request = await _db.Requests.FirstOrDefaultAsync(r => r.RequestId == RequestId);
            _db.Requests.Remove(request);
            _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { userId = UserId });
        }


        [HttpPost("home/report/{RequestId}")]
        public async Task<IActionResult> Report(int RequestId, DateOnly StartDate, DateOnly EndDate, string Reason, string RequestType, string Status, int UserId){
            var user = await _db.Users.Include(u => u.Requests).FirstOrDefaultAsync(u => u.UserId == UserId);
            if (RequestId != 0)
            {
                var request = await _db.Requests.Include(u => u.User).FirstOrDefaultAsync(r => r.RequestId == RequestId);
                
                request.StartDate = StartDate;
                request.EndDate = EndDate;
                request.Reason = Reason;
                request.RequestType = RequestType;
                request.Status = Status;
                request.User = user;
            }
            else
            {          
                var request = new Request
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Reason = Reason,
                    RequestType = RequestType,
                    Status = Status,
                    UserId = UserId
                };
                user.Requests.Add(request);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { userId = UserId });

        }
    }
}
