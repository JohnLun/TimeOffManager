using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeOffManager.Models;
using TimeOffManager.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using BCrypt.Net; //import hashing dependency

namespace TimeOffManager.Controllers
{
    public class LoginController : Controller
    {
        private readonly URDbContext _db;


        public LoginController(URDbContext dbContext)
        {
            _db = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
         public async Task<IActionResult> Login(string Email, string Password)
             {
  

             var user = await _db.Users.FirstOrDefaultAsync(usr => usr.Email == Email);

             if (user == null)
             {
                 Console.WriteLine("User not found.");
                 ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
                 return View("Index");
             }


             bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);
             Console.WriteLine($"Password Valid: {isPasswordValid}");

             // Check if the password is correct
             if (isPasswordValid)
             {
             // Successful login: set session variables
                 HttpContext.Session.SetString("Firstname", user.FirstName);
                 HttpContext.Session.SetString("LastName", user.LastName);
                 HttpContext.Session.SetInt32("UserId", user.UserId);

                 // Redirect to the home page
                 Console.WriteLine("Login successful! Redirecting to home.");
                 return RedirectToAction("Index", "Home");
             }

             // If login fails, set an error message
             Console.WriteLine("Invalid password.");
             ViewBag.ErrorMessage = "Invalid email or password. Please try again.";

             // Return the login view
             return View("Index");
             }
        [HttpGet("create-account")]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost("create-account")]
        public IActionResult CreateAccount(string FirstName, string LastName, string Email, string Password)
        {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password); //hashed password var
                // Create a new user object
                var user = new User
                {
                    Email = Email,
                    Password = hashedPassword, // In a real application, use a hashing function
                    FirstName = FirstName,
                    LastName = LastName,
                    // Initialize PTOHours, CIHours, FHHours with default values
                    PTOHours = 160,
                    CIHours = 16,
                    FHHours = 8
                };

                // Add the user to the database
                _db.Users.Add(user);
                _db.SaveChanges();

                // Redirect to the home index view
                return RedirectToAction("Index", "Login");
        }
    }
}
