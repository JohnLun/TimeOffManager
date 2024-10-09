using Microsoft.EntityFrameworkCore;
using TimeOffManager.Models;

namespace TimeOffManager.Data
{
    public class URDbContext : DbContext
    {
        public URDbContext(DbContextOptions<URDbContext> options)
       : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
