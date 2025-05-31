using Microsoft.EntityFrameworkCore;
using LogTakipAPI.Models;

namespace LogTakipAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
