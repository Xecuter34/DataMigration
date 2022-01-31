using DataMigration.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace DataMigration.DB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<UserOldProfile> UserOldProfiles { get; set; }
        public DbSet<Creator> Creators { get; set; }
        public DbSet<CreatorSocialAccount> CreatorSocialAccounts { get; set; }
        public DbSet<CreatorSocialRefresh> CreatorSocialRefreshes { get; set; }
        public DbSet<CreatorSocialRefreshData> CreatorSocialRefreshData { get; set; }
        public DbSet<SocialPlatform> SocialPlatforms { get; set; }
        public DbSet<SocialAccountPost> SocialAccountPosts { get; set; }
        public DbSet<SocialAccountPostStatHistory> SocialAccountPostStatsHistory { get; set; }
        public DbSet<SocialAccountPostStatLatest> SocialAccountPostStatsLatest { get; set; }
        public DbSet<SocialAccountStatHistory> SocialAccountStatsHistory { get; set; }
        public DbSet<SocialAccountStatLatest> SocialAccountStatsLatest { get; set; }
        public DbSet<SocialAccountStatMetrics> SocialAccountStatsMetrics { get; set; }
        public DbSet<AccountsOldProfile> AccountsOldProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrganisationCreator> OrganisationCreators { get; set; }
        public DbSet<OrganisationOldProfile> OrganisationOldProfiles { get; set; }
        public DbSet<SocialAccountPostStatMetrics> SocialAccountPostStatsMetrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql($"Host={Environment.GetEnvironmentVariable("Host")};" +
                    $"Port={Environment.GetEnvironmentVariable("Port")};" +
                    $"Database={Environment.GetEnvironmentVariable("Database")};" +
                    $"Username={Environment.GetEnvironmentVariable("Username")};" +
                    $"Password={Environment.GetEnvironmentVariable("Password")}")
                .UseSnakeCaseNamingConvention()
                .LogTo(Console.WriteLine, LogLevel.Information);
    }
    
}
