using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DataMigration.DB;
using DataMigration.DB.Models;

namespace DataMigration
{
    class Migrations
    {
        public async Task MigrateUsersAsync(string userData)
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

                string verifiedAt = users[i].verifiedAt?.date;
                string createdAt = users[i].createdAt?.date;
                string updatedAt = users[i].updatedAt?.date;

                dbContext.Add(new User
                {
                    Id = newUserId,
                    Email = users[i].email,
                    Password = users[i].password,
                    FirstName = users[i].firstName,
                    LastName = users[i].lastName,
                    TermsSigned = users[i].termsSigned,
                    SignupPreferenceId = users[i].signupPreference == "creator" ? 1 : 2,
                    VerifiedAt = Convert.ToDateTime(verifiedAt),
                    ValidationToken = users[i].validationToken,
                    CreatedAt = Convert.ToDateTime(createdAt),
                    UpdatedAt = Convert.ToDateTime(updatedAt),
                    AddressId = null
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
