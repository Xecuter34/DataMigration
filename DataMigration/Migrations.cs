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

                dbContext.Add(new UserProfile
                {
                    UserId = newUserId,
                    Dob = dob,
                    Username = users[i].username
                });
                await dbContext.SaveChangesAsync();

                dbContext.Add(new User
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

                dbContext.Add(new UserOldProfile
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
                dbContext.Add(new Organization
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
                        dbContext.Add(new OrganizationUser
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

        public static async Task MigrateAccountsAsync(string trackedSocialData, string accountsData)
        {
            if (trackedSocialData == null || accountsData == null) return;

            List<OldModel.Accounts> accounts = JsonSerializer.Deserialize<List<OldModel.Accounts>>(accountsData);
            List<OldModel.TrackedSocials> trackedSocials = JsonSerializer.Deserialize<List<OldModel.TrackedSocials>>(trackedSocialData);
            using DatabaseContext dbContext = new DatabaseContext();
            using ProgressBar progressBar = new ProgressBar();

            Console.Write($"Migrating accounts data... ");
            for (int i = 0; i < accounts.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);

                OldModel.TrackedSocials trackedSocial = trackedSocials.Find(t => t.connectionId.oid == accounts[i]._id.oid);

                Guid creatorId = Guid.NewGuid();
                // Rest of schema doesn't exist on old/can't create value for it...
                dbContext.Add(new Creator
                {
                    Id = creatorId,
                    UserId = Guid.Parse(accounts[i].uniqueIdentifiers.oauthLookupId)
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

                dbContext.Add(new CreatorSocialAccount
                {
                    CreatorId = creatorId,
                    // TODO: Find out what the enum looks like and which platform has which ID.
                    SocialPlatformId = socialPlatformId,
                    // TODO: Check to see if needs changing (What is the enum going to look like?)
                    Status = accounts[i].archived 
                        ? (int)Enum.AccountStatus.ARCHIVED 
                        : (int)Enum.AccountStatus.ACTIVE,
                    Name = accounts[i].meta.name,
                    Avatar = accounts[i].meta.avatar,
                    Token = accounts[i].uniqueIdentifiers.oauthLookupId,
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

            for (int i = 0; i < posts.Count; i++)
            {
                progressBar.Report((double)(i + 1) / 100);
                Guid newPostId = Guid.NewGuid();

                // TODO: Add the rest of the logic once we know where everything is going
                dbContext.Add(new SocialAccountPost
                {
                    Id = newPostId
                });

                Console.WriteLine(posts[i]);
            }
        }
    }
}
