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
            List<OldModel.Users> users = JsonSerializer.Deserialize<List<OldModel.Users>>(userData);
            using DatabaseContext dbContext = new DatabaseContext();

            for (int i = 0; i < users.Count; i++)
            {
                Console.Write($"\rMigrating user to users [{i + 1}/{users.Count}]");
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

            Console.WriteLine("\nSuccessfully migrated users.");
        }

        public static async Task MigrateOrganisationAsync(string orgData)
        {
            List<OldModel.Organizations> orgs = JsonSerializer.Deserialize<List<OldModel.Organizations>>(orgData);
            using DatabaseContext dbContext = new DatabaseContext();

            for (int i = 0; i < orgs.Count; i++)
            {
                Console.Write($"\rMigrating org to organizations [{i + 1}/{orgs.Count}]");
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

            Console.WriteLine("\nSuccessfully migrated organizations.");
        }
    }
}
