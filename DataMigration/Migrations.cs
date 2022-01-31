#nullable enable
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
                progressBar.Report((double)(i + 1) / users.Count);

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

            Console.WriteLine(" Done.");
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
                progressBar.Report((double)(i + 1) / orgs.Count);

                Guid newOrgId = Guid.NewGuid();
                await dbContext.AddAsync(new Organisation
                {
                    Id = newOrgId,
                    Name = orgs[i].name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();

                await dbContext.AddAsync(new OrganisationOldProfile
                {
                    NewOrgId = newOrgId,
                    OldOrgId = orgs[i]._id.oid
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

            Console.WriteLine(" Done.");
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
                progressBar.Report((double)(i + 1) / accounts.Count);

                OldModel.TrackedSocials? trackedSocial = trackedSocials.Find(t => {
                    if (t.connectionId != null)
                    {
                        return t.connectionId.oid == accounts[i]._id.oid;
                    }
                    return false;
                });
                OldModel.OAuthFlowStorages? oAuthFlowStorage = oauthFlowStorages.Find(o => o.platformUserId == accounts[i].uniqueIdentifiers.platformUserId);

                Guid creatorId = Guid.NewGuid();
                Guid t = dbContext.UserOldProfiles.Where(user => user.OldUserId == accounts[i].uniqueIdentifiers.oauthLookupId).First().NewUserId;
                List<Creator> x =  dbContext.Creators.Where(y => y.UserId == t).ToList();
                if (x.Count() == 0)
                {
                    await dbContext.AddAsync(new AccountsOldProfile
                    {
                        NewAccountId = creatorId,
                        OldAccountId = accounts[i]._id.oid,
                        Platform = accounts[i].platform
                    });
                    await dbContext.SaveChangesAsync();

                    Guid addressId = await Utils.Address.AddAddressAsync();
                    await dbContext.AddAsync(new Creator
                    {
                        Id = creatorId,
                        UserId = t,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        // TODO: Adding generated value until we have implemented this (might just need work on migration tool).
                        AddressId = addressId
                    });
                    await dbContext.SaveChangesAsync();

                    if (trackedSocial != null)
                    {
                        await dbContext.AddAsync(new OrganisationCreator
                        {
                            OrganisationId = dbContext.OrganisationOldProfiles.Where(o => o.OldOrgId == trackedSocial.org.oid).FirstOrDefault().NewOrgId,
                            CreatorId = creatorId,
                            CreatedAt = DateTime.Parse(trackedSocial.createdAt.date),
                            UpdatedAt = DateTime.UtcNow
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    creatorId = x[0].Id;
                }

                await dbContext.AddAsync(new CreatorSocialAccount
                {
                    CreatorId = creatorId,
                    Status = "Active",
                    PlatformUniqueIdentifier = accounts[i].uniqueIdentifiers.platformUserId ?? "NOT_SUPPLIED",
                    Name = accounts[i].meta.name ?? "NOT_SUPPLIED",
                    Avatar = accounts[i].meta.avatar,
                    Token = oAuthFlowStorage?.accessToken,
                    RefreshToken = oAuthFlowStorage?.refreshToken,
                    Platform = accounts[i].platform,
                    NextUpdateAt = Convert.ToDateTime(accounts[i].nextUpdateDue?.date),
                    CreatedAt = Convert.ToDateTime(accounts[i].connectedOn?.date),
                    UpdatedAt = Convert.ToDateTime(accounts[i].updatedAt?.date)
                });
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine(" Done.");
        }

        public static async Task MigratePostAsync(string postsData, string detailedPostClusterData)
        {
            if (postsData == null || detailedPostClusterData == null) return;
            //await Validator.ValidateSocialPlatformsAsync();

            List<OldModel.Posts> posts = JsonSerializer.Deserialize<List<OldModel.Posts>>(postsData);
            List<OldModel.DetailedPostsCluster> detailedPostClusters = JsonSerializer.Deserialize<List<OldModel.DetailedPostsCluster>>(detailedPostClusterData);

            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.WriteLine(posts.Count);

            Console.Write($"Migrating posts data... ");
            for (int i = 0; i < posts.Count; i++)
            {
                progressBar.Report((double)(i + 1) / posts.Count);
                OldModel.DetailedPostsCluster? detailedPostCluster = detailedPostClusters.FirstOrDefault(p => p.postId.oid == posts[i]._id.oid);
                Guid newPostId = Guid.NewGuid();
                List<AccountsOldProfile> accounts = dbContext.AccountsOldProfiles.Where(account => account.OldAccountId == posts[i].accountId).ToList();

                if (accounts.Count > 0)
                {
                    AccountsOldProfile account = accounts[0];
                    List<CreatorSocialAccount> creatorSocialAccounts = dbContext.CreatorSocialAccounts.Where(a => a.CreatorId == account.NewAccountId).ToList();

                    if (detailedPostCluster != null && creatorSocialAccounts.Count > 0)
                    {
                        CreatorSocialAccount creatorSocialAccount = creatorSocialAccounts.First(c => c.Platform == account.Platform);
                        if (dbContext.SocialAccountPosts.FirstOrDefault(s => s.CreatorSocialAccountId == creatorSocialAccount.Id && s.SocialMedialUid == posts[i].platformPostId) != null) continue;
                        await dbContext.AddAsync(new SocialAccountPost
                        {
                            Id = newPostId,
                            CreatorSocialAccountId = creatorSocialAccount.Id,
                            SocialMedialUid = posts[i].platformPostId,
                            Title = posts[i].primaryText,
                            Description = posts[i].secondaryText,
                            ImageUrl = posts[i].imageUrl,
                            PublishedAt = Convert.ToDateTime(posts[i].createdAt?.date),
                            CreatedAt = Convert.ToDateTime(posts[i].createdAt?.date),
                            UpdatedAt = Convert.ToDateTime(posts[i].updatedAt?.date)
                        });
                        await dbContext.SaveChangesAsync();

                        for (int ii = 0; ii < detailedPostCluster.slices.Count(); ii++)
                        {
                            Slices slice = detailedPostCluster.slices[ii];
                            CreatorSocialRefresh newCreatorSocialRefresh = new CreatorSocialRefresh
                            {
                                CreatorSocialAccountId = creatorSocialAccount.Id,
                                StartedAt = Convert.ToDateTime(detailedPostCluster.startDate?.date),
                                FinishedAt = Convert.ToDateTime(detailedPostCluster.endDate?.date),
                                SocialAccountRefreshStatusId = (int)Enum.RefreshStatus.COMPLETED,
                                CreatedAt = Convert.ToDateTime(detailedPostCluster.startDate?.date),
                                UpdatedAt = DateTime.UtcNow,
                                NumApiCalls = 1
                            };
                            await dbContext.AddAsync(newCreatorSocialRefresh);
                            await dbContext.SaveChangesAsync();

                            foreach (var data in slice.data)
                            {
                                double value = .0f;
                                try
                                {
                                    value = data.Value.GetDouble();
                                }
                                catch (Exception)
                                {
                                    // Failed to convert the value for some reason, move on to the next one.
                                    continue;
                                }
                                List<SocialAccountPostStatMetrics> socialAccountPostStatMetrics = dbContext.SocialAccountPostStatsMetrics.Where(s => s.Slug == data.Key).ToList();
                                SocialAccountPostStatMetrics socialAccountPostStatMetric;
                                if (socialAccountPostStatMetrics.Count() == 0)
                                {
                                    socialAccountPostStatMetric = new SocialAccountPostStatMetrics
                                    {
                                        Slug = data.Key.ToString(),
                                        Name = data.Key.ToString(),
                                        IsDeleted = false,
                                        CreatedAt = Convert.ToDateTime(detailedPostCluster.startDate?.date),
                                        UpdatedAt = DateTime.UtcNow
                                    };
                                    await dbContext.AddAsync(socialAccountPostStatMetric);
                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    socialAccountPostStatMetric = socialAccountPostStatMetrics[0];
                                }
                                

                                SocialAccountPostStatHistory socialAccountPostStatHistory = new SocialAccountPostStatHistory
                                {
                                    CreatorSocialRefreshId = newCreatorSocialRefresh.Id,
                                    Value = value,
                                    CreatedAt = Convert.ToDateTime(slice.collectedAt?.date),
                                    SocialAccountPostStatsMetricId = socialAccountPostStatMetric.Id,
                                    SocialAccountPostId = newPostId
                                };
                                await dbContext.AddAsync(socialAccountPostStatHistory);
                                await dbContext.SaveChangesAsync();

                                if (detailedPostCluster.slices[detailedPostCluster.slices.Count() - 1]._id.oid == detailedPostCluster._id.oid)
                                {
                                    SocialAccountPostStatLatest socialAccountPostStatLatest = new SocialAccountPostStatLatest
                                    {
                                        Value = value,
                                        CreatedAt = Convert.ToDateTime(slice.collectedAt?.date),
                                        UpdatedAt = DateTime.UtcNow,
                                        SocialAccountPostStatsMetricId = socialAccountPostStatMetric.Id,
                                        SocialAccountPostId = newPostId
                                    };
                                    await dbContext.AddAsync(socialAccountPostStatLatest);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(" Done.");
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
                progressBar.Report((double)(i + 1) / detailedStatsClusters.Count);
                List<AccountsOldProfile> accounts = dbContext.AccountsOldProfiles.Where(account => account.OldAccountId == detailedStatsClusters[i].accountId.oid).ToList();

                if (accounts.Count > 0)
                {
                    AccountsOldProfile account = accounts[0];
                    List<CreatorSocialAccount> creatorSocialAccounts = dbContext.CreatorSocialAccounts.Where(a => a.CreatorId == account.NewAccountId).ToList();
                    OldModel.DetailedStatsCluster? detailedStatCluster = detailedStatsClusters.Find(p => p._id.oid == detailedStatsClusters[i]._id.oid);

                    if (detailedStatCluster != null && creatorSocialAccounts.Count > 0)
                    {
                        CreatorSocialAccount creatorSocialAccount = creatorSocialAccounts.First(c => c.Platform == account.Platform);
                        foreach (Slices slice in detailedStatsClusters[i].slices)
                        {
                            CreatorSocialRefresh newCreatorSocialRefresh = new CreatorSocialRefresh
                            {
                                CreatorSocialAccountId = creatorSocialAccount.Id,
                                StartedAt = Convert.ToDateTime(detailedStatCluster.startDate?.date),
                                FinishedAt = Convert.ToDateTime(detailedStatCluster.endDate?.date),
                                SocialAccountRefreshStatusId = (int)Enum.RefreshStatus.COMPLETED,
                                CreatedAt = Convert.ToDateTime(detailedStatCluster.startDate?.date),
                                UpdatedAt = DateTime.UtcNow,
                                NumApiCalls = 1
                            };
                            await dbContext.AddAsync(newCreatorSocialRefresh);
                            await dbContext.SaveChangesAsync();

                            foreach (var data in slice.data)
                            {
                                double value = .0f;
                                try
                                {
                                    value = data.Value.GetDouble();
                                }
                                catch (Exception)
                                {
                                    // Failed to convert the value for some reason, move on to the next one.
                                    continue;
                                }
                                List<SocialAccountStatMetrics> socialAccountStatMetrics = dbContext.SocialAccountStatsMetrics.Where(s => s.Slug == data.Key).ToList();
                                SocialAccountStatMetrics socialAccountStatMetric;
                                if (socialAccountStatMetrics.Count() == 0)
                                {
                                    socialAccountStatMetric = new SocialAccountStatMetrics
                                    {
                                        Slug = data.Key.ToString(),
                                        Name = data.Key.ToString(),
                                        IsDeleted = false,
                                        CreatedAt = Convert.ToDateTime(detailedStatCluster.startDate?.date),
                                        UpdatedAt = DateTime.UtcNow
                                    };
                                    await dbContext.AddAsync(socialAccountStatMetric);
                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    socialAccountStatMetric = socialAccountStatMetrics[0];
                                }

                                SocialAccountStatHistory socialAccountStatHistory = new SocialAccountStatHistory
                                {
                                    CreatorSocialRefreshId = newCreatorSocialRefresh.Id,
                                    Value = value,
                                    CreatedAt = Convert.ToDateTime(slice.collectedAt?.date),
                                    SocialAccountStatsMetricId = socialAccountStatMetric.Id
                                };
                                await dbContext.AddAsync(socialAccountStatHistory);
                                await dbContext.SaveChangesAsync();

                                if (detailedStatsClusters[i].slices[detailedStatsClusters[i].slices.Count() - 1]._id.oid == detailedStatCluster._id.oid)
                                {
                                    SocialAccountStatLatest socialAccountStatLatest = new SocialAccountStatLatest
                                    {
                                        CreatorSocialAccountId = account.NewAccountId,
                                        SocialAccountStatsMetricId = socialAccountStatMetric.Id,
                                        Value = value,
                                        CreatedAt = Convert.ToDateTime(slice.collectedAt?.date),
                                        UpdatedAt = DateTime.UtcNow
                                    };
                                    await dbContext.AddAsync(socialAccountStatLatest);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(" Done.");
        }
    }
}
