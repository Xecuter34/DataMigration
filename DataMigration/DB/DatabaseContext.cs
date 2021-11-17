using DataMigration.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataMigration.DB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<UserOldProfile> UserOldProfiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql($"Host={Environment.GetEnvironmentVariable("Host")};" +
                    $"Port={Environment.GetEnvironmentVariable("Port")};" +
                    $"Database={Environment.GetEnvironmentVariable("Database")};" +
                    $"Username={Environment.GetEnvironmentVariable("Username")};" +
                    $"Password={Environment.GetEnvironmentVariable("Password")}")
                .UseSnakeCaseNamingConvention();
    }
}
