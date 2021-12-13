using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DataMigration.DB;
using DataMigration.DB.Models;
using DataMigration.OldModel.Generics;
using DataMigration.Utils;

namespace DataMigration
{
    class Migrations
    {
        public static async Task MigrateUsersAsync(string userData)
        {
            if (userData == null) return;
            List<OldModel.Users> users = Validator.GetValidUsersAsync(JsonSerializer.Deserialize<List<OldModel.Users>>(userData));
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

                //await dbContext.AddAsync(new UserProfile
                //{
                //    UserId = newUserId,
                //    Dob = dob,
                //    Username = users[i].username
                //});
                //await dbContext.SaveChangesAsync();

                await dbContext.AddAsync(new User
                {
                    Id = newUserId,
                    Email = users[i].email ?? "NOT_SUPPLIED",
                    Password = users[i].password ?? "NOT_SUPPLIED",
                    FirstName = users[i].firstName ?? "NOT_SUPPLIED",
                    LastName = users[i].lastName ?? "NOT_SUPPLIED",
                    CreatedAt = Convert.ToDateTime(users[i].createdAt?.date),
                    UpdatedAt = Convert.ToDateTime(users[i].updatedAt?.date),
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
            List<OldModel.Organisations> orgs = JsonSerializer.Deserialize<List<OldModel.Organisations>>(orgData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating Organisation data... ");
            for (int i = 0; i < orgs.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);

                Guid newOrgId = Guid.NewGuid();
                await dbContext.AddAsync(new Organisation
                {
                    Id = newOrgId,
                    Name = orgs[i].name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
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
                        await dbContext.AddAsync(new OrganisationUser
                        {
                            UserId = userOldProfiles[0],
                            OrganisationId = newOrgId,
                            IsOwner = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
                await dbContext.SaveChangesAsync();
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

                Guid t = dbContext.UserOldProfiles.Where(user => user.OldUserId == accounts[i].uniqueIdentifiers.oauthLookupId).First().NewUserId;
                // Rest of schema doesn't exist on old/can't create value for it...
                List<Creator> x =  dbContext.Creators.Where(y => y.UserId == t).ToList();
                if (x.Count() == 0)
                {
                    await dbContext.AddAsync(new Creator
                    {
                        Id = creatorId,
                        UserId = t,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        // Adding static value until we have implemented this.
                        AddressId = Guid.Parse("82c5e076-9a87-401b-834f-2959d04c7b70")
                    });
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    creatorId = x[0].Id;
                }

                int socialPlatformId = accounts[i].platform switch
                {
                    "instagram" => (int)Enum.SocialPlatforms.INSTAGRAM,
                    "twitter" => (int)Enum.SocialPlatforms.TWITTER,
                    "facebook" => (int)Enum.SocialPlatforms.FACEBOOK,
                    "youtube" => (int)Enum.SocialPlatforms.YOUTUBE,
                    "twitch" => (int)Enum.SocialPlatforms.TWITCH,
                    _ => 0
                };

                await dbContext.AddAsync(new CreatorSocialAccount
                {
                    CreatorId = creatorId,
                    // TODO: Update this when fixed on the migration.
                    SocialPlatformId = socialPlatformId,
                    PlatformUniqueIdentifier = accounts[i].uniqueIdentifiers.platformUserId,
                    Name = accounts[i].meta.name,
                    Avatar = accounts[i].meta.avatar,
                    Token = oAuthFlowStorage.accessToken,
                    RefreshToken = oAuthFlowStorage.refreshToken,
                    SocialAccountStatusId = 1,
                    CreatedAt = Convert.ToDateTime(accounts[i].connectedOn?.date),
                    UpdatedAt = Convert.ToDateTime(accounts[i].updatedAt?.date)
                });
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Done.");
        }

        public static async Task MigratePostAsync(string postsData, string detailedPostClusterData)
        {
            if (postsData == null || detailedPostClusterData == null) return;
            await Utils.Validator.ValidateSocialPlatformsAsync();

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
                    // TODO: Update for any new changes
                    SocialMediaUid = "",
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
                            // TODO: Update for any new changes
                            // Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Twitter Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "instagram":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.INSTAGRAM,
                            // TODO: Update for any new changes
                            // Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Instagram Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "twitch":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.TWITCH,
                            // TODO: Update for any new changes
                            // Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"Twitch Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "youtube":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.YOUTUBE,
                            // TODO: Update for any new changes
                            // Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
                            Name = $"YouTube Post - {posts[i].platformPostId}"
                        });
                        break;
                    case "facebook":
                        await dbContext.AddAsync(new SocialAccountStatMetrics
                        {
                            SocialPlatformId = (int)Enum.SocialPlatforms.FACEBOOK,
                            // TODO: Update for any new changes
                            // Slug = $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}",
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
                        //SocialAccountStatMetricId = dbContext.SocialAccountStatMetrics
                        //    .Where(metric => metric.Slug == $"{creatorSocialAccount.SocialPlatformUserId}_{posts[i].platformPostId}")
                        //    .First().Id,
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
                    // Slug = $"{creatorSocialAccount.SocialPlatformUserId}",
                    Name = $"Account Stat - {detailedStatsClusters[i].accountId.oid}"
                });
                await dbContext.SaveChangesAsync();

                foreach (Slices slice in detailedStatsClusters[i].slices)
                {
                    await dbContext.AddAsync(new SocialAccountStatHistory
                    {
                        //SocialAccountStatMetricId = dbContext.SocialAccountStatMetrics
                        //    .Where(metric => metric.Slug == $"{creatorSocialAccount.SocialPlatformUserId}")
                        //    .Last().Id,
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
