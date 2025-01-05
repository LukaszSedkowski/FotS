using Microsoft.EntityFrameworkCore;
using API_Fall_Of_The_Sun.Models;

namespace API_Fall_Of_The_Sun.Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<HallOfFame> HallOfFame { get; set; }
    }
}
