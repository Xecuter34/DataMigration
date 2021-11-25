using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DataMigration.DB;
using DataMigration.DB.Models;
using DataMigration.OldModel.Generics;

namespace DataMigration
{
    class Migrations
    {
        public static async Task MigrateUsersAsync(string userData)
        {
            if (userData == null) return;
            List<OldModel.Users> users = JsonSerializer.Deserialize<List<OldModel.Users>>(userData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating user data... ");
            for (int i = 0; i < users.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);

                Guid newUserId = Guid.NewGuid();
                DateTime dob = new DateTime();

                try
                {
                    if (users[i].dob != null)
                    {
                        dob = Convert.ToDateTime(users[i].dob.date.ToString());
                    }
                }
                catch (Exception) { }

                await dbContext.AddAsync(new UserProfile
                {
                    UserId = newUserId,
                    Dob = dob,
                    Username = users[i].username
                });
                await dbContext.SaveChangesAsync();

                await dbContext.AddAsync(new User
                {
                    Id = newUserId,
                    Email = users[i].email,
                    Password = users[i].password,
                    FirstName = users[i].firstName,
                    LastName = users[i].lastName,
                    TermsSigned = users[i].termsSigned,
                    SignupPreferenceId = users[i].signupPreference == "creator" ? 1 : 2,
                    VerifiedAt = Convert.ToDateTime(users[i].verifiedAt?.date),
                    ValidationToken = users[i].validationToken,
                    CreatedAt = Convert.ToDateTime(users[i].createdAt?.date),
                    UpdatedAt = Convert.ToDateTime(users[i].updatedAt?.date),
                    AddressId = null
                });
                await dbContext.SaveChangesAsync();

                await dbContext.AddAsync(new UserOldProfile
                {
                    NewUserId = newUserId,
                    OldUserId = users[i]._id.oid
                });
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Done.");
        }

        public static async Task MigrateOrganisationsAsync(string orgData)
        {
            if (orgData == null) return;
            List<OldModel.Organizations> orgs = JsonSerializer.Deserialize<List<OldModel.Organizations>>(orgData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating organization data... ");
            for (int i = 0; i < orgs.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);

                Guid newOrgId = Guid.NewGuid();
                await dbContext.AddAsync(new Organization
                {
                    Id = newOrgId,
                    Name = orgs[i].name,
                    Validated = orgs[i].validated != null && (bool) orgs[i].validated,
                    Logo = orgs[i].logo,
                    WhiteLabelEnabled = orgs[i].whiteLabelEnabled != null && (bool) orgs[i].whiteLabelEnabled
                });
                await dbContext.SaveChangesAsync();

                foreach (ObjectId admin in orgs[i].admins)
                {
                    List<Guid> userOldProfiles = dbContext.UserOldProfiles
                        .Where(u => u.OldUserId == admin.oid)
                        .Select(u => u.NewUserId)
                        .ToList();

                    if (userOldProfiles.Count > 0)
                    {
                        await dbContext.AddAsync(new OrganizationUser
                        {
                            UserId = userOldProfiles[0],
                            OrganizationId = newOrgId
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            Console.WriteLine("Done.");
        }

        public static async Task MigrateAccountsAsync(string trackedSocialData, string accountsData, string oauthFlowStoragesData)
        {
            if (trackedSocialData == null || accountsData == null || oauthFlowStoragesData == null) return;

            List<OldModel.Accounts> accounts = JsonSerializer.Deserialize<List<OldModel.Accounts>>(accountsData);
            List<OldModel.TrackedSocials> trackedSocials = JsonSerializer.Deserialize<List<OldModel.TrackedSocials>>(trackedSocialData);
            List<OldModel.OAuthFlowStorages> oauthFlowStorages = JsonSerializer.Deserialize<List<OldModel.OAuthFlowStorages>>(oauthFlowStoragesData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating accounts data... ");
            for (int i = 0; i < accounts.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);

                OldModel.TrackedSocials trackedSocial = trackedSocials.Find(t => t.connectionId.oid == accounts[i]._id.oid);
                OldModel.OAuthFlowStorages oAuthFlowStorage = oauthFlowStorages.Find(o => o.platformUserId == accounts[i].uniqueIdentifiers.platformUserId);

                Guid creatorId = Guid.NewGuid();
                await dbContext.AddAsync(new AccountsOldProfile
                {
                    NewAccountId = creatorId,
                    OldAccountId = accounts[i]._id.oid
                });
                await dbContext.SaveChangesAsync();

                // Rest of schema doesn't exist on old/can't create value for it...
                await dbContext.AddAsync(new Creator
                {
                    Id = creatorId,
                    UserId = dbContext.UserOldProfiles.Where(user => user.OldUserId == accounts[i].uniqueIdentifiers.oauthLookupId).First().NewUserId
                });
                await dbContext.SaveChangesAsync();

                int socialPlatformId = 0;
                switch (accounts[i].platform)
                {
                    case "instagram":
                        socialPlatformId = (int)Enum.SocialPlatforms.INSTAGRAM;
                        break;
                    case "twitter":
                        socialPlatformId = (int)Enum.SocialPlatforms.TWITTER;
                        break;
                    case "facebook":
                        socialPlatformId = (int)Enum.SocialPlatforms.FACEBOOK;
                        break;
                    case "youtube":
                        socialPlatformId = (int)Enum.SocialPlatforms.YOUTUBE;
                        break;
                    case "twitch":
                        socialPlatformId = (int)Enum.SocialPlatforms.TWITCH;
                        break;
                }

                await dbContext.AddAsync(new CreatorSocialAccount
                {
                    CreatorId = creatorId,
                    // TODO: Find out what the enum looks like and which platform has which ID.
                    SocialPlatformId = socialPlatformId,
                    SocialPlatformUserId = accounts[i].uniqueIdentifiers.platformUserId,
                    // TODO: Check to see if needs changing (What is the enum going to look like?)
                    Status = accounts[i].archived 
                        ? (int)Enum.AccountStatus.ARCHIVED 
                        : (int)Enum.AccountStatus.ACTIVE,
                    Name = accounts[i].meta.name,
                    Avatar = accounts[i].meta.avatar,
                    Token = oAuthFlowStorage.accessToken,
                    RefreshToken = oAuthFlowStorage.refreshToken,
                    ConnectedAt = Convert.ToDateTime(accounts[i].connectedOn?.date),
                    UpdatedAt = Convert.ToDateTime(accounts[i].updatedAt?.date)
                });
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Done.");
        }

        public static async Task MigratePostAsync(string postsData, string detailedPostClusterData)
        {
            if (postsData == null || detailedPostClusterData == null) return;

            List<OldModel.Posts> posts = JsonSerializer.Deserialize<List<OldModel.Posts>>(postsData);
            List<OldModel.DetailedPostsCluster> detailedPostClusters = JsonSerializer.Deserialize<List<OldModel.DetailedPostsCluster>>(detailedPostClusterData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating posts data... ");
            for (int i = 0; i < posts.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);
                OldModel.DetailedPostsCluster detailedPostCluster = detailedPostClusters.Find(p => p.postId.oid == posts[i]._id.oid);
                Guid newPostId = Guid.NewGuid();
                Guid accountId = dbContext.AccountsOldProfiles.Where(account => account.OldAccountId == posts[i].accountId).First().NewAccountId;
                CreatorSocialAccount creatorSocialAccount = dbContext.CreatorSocialAccounts.Where(account => account.Id == accountId).First();

                // TODO: Add the rest of the logic once we know where everything is going
                await dbContext.AddAsync(new SocialAccountPost
                {
                    Id = newPostId,
                    CreatorSocialAccountId = accountId,
                    SocialMediaUid = creatorSocialAccount.SocialPlatformUserId,
                    CreatedAt = Convert.ToDateTime(posts[i].createdAt?.date)
                });
                await dbContext.SaveChangesAsync();

                await dbContext.AddAsync(new CreatorSocialRefresh
                {
                    CreatorSocialAccountId = creatorSocialAccount.Id,
                    StartAt = Convert.ToDateTime(detailedPostCluster.startDate?.date),
                    EndAt = Convert.ToDateTime(detailedPostCluster.endDate?.date),
                    Status = (int)Enum.RefreshStatus.COMPLETED,
                    NumOfApiCalls = 1
                });
                await dbContext.SaveChangesAsync();

                switch (posts[i].platform)
                {
                    case "twitter":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.TWITTER,
                            Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Twitter Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "instagram":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.INSTAGRAM,
                            Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Instagram Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "twitch":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.TWITCH,
                            Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Twitch Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "youtube":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.YOUTUBE,
                            Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"YouTube Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "facebook":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.FACEBOOK,
                            Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Facebook Post - {posts[i].platformPostId}"
                        });
                        break;
                }
                await dbContext.SaveChangesAsync();

                foreach (Slices slice in detailedPostCluster.slices)
                {
                    await dbContext.AddAsync(new SocialAccountPostStatHistory
                    {
                        SocialAccountPostId = newPostId,
                        SocialAccountStatMetricId = dbContext.SocialAccountStatMetrics
                            .Where(metric => metric.Slug == $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}")
                            .First().Id,
                        CreatorSocialRefreshId = dbContext.CreatorSocialRefreshes
                            .Where(account => account.CreatorSocialAccountId == creatorSocialAccount.Id)
                            .Last().Id,
                        CollectedAt = Convert.ToDateTime(posts[i].createdAt?.date)
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Done.");
        }

        public static async Task MigrateStatsAsync(string detailedStatClusterData)
        {
            if (detailedStatClusterData == null) return;

            List<OldModel.DetailedStatsCluster> detailedStatsClusters = JsonSerializer.Deserialize<List<OldModel.DetailedStatsCluster>>(detailedStatClusterData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating stats data... ");
            for (int i = 0; i < detailedStatsClusters.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);
                Guid accountId = dbContext.AccountsOldProfiles.Where(account => account.OldAccountId == detailedStatsClusters[i].accountId.oid).First().NewAccountId;
                CreatorSocialAccount creatorSocialAccount = dbContext.CreatorSocialAccounts.Where(account => account.Id == accountId).First();

                await dbContext.AddAsync(new SocialAccountStatMetrics
                {
                    SocialPlatformId = creatorSocialAccount.SocialPlatformId,
                    Slug = $"{creatorSocialAccount.SocialPlatformUserId}",
                    Name = $"Account Stat - {detailedStatsClusters[i].accountId.oid}"
                });
                await dbContext.SaveChangesAsync();

                foreach (Slices slice in detailedStatsClusters[i].slices)
                {
                    await dbContext.AddAsync(new SocialAccountStatHistory
                    {
                        SocialAccountStatMetricId = dbContext.SocialAccountStatMetrics
                        .Where(metric => metric.Slug == $"{creatorSocialAccount.SocialPlatformUserId}")
                        .Last().Id,
                        CreatorSocialRefreshId = dbContext.CreatorSocialRefreshes
                        .Where(account => account.CreatorSocialAccountId == creatorSocialAccount.Id)
                        .Last().Id,
                        CollectedAt = Convert.ToDateTime(detailedStatsClusters[i].startDate?.date)
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Done.");
        }
    }
}
