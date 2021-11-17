using DataMigration.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.DB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql("Host=localhost;Port=5433;Database=edge_dev;Username=postgres;Password=sJvCv34QYVAy")
                .UseSnakeCaseNamingConvention();
    }
}
