using DataMigration.DB;
using DataMigration.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigration.Enum;
using System.Linq;

namespace DataMigration.Utils
{
    class Validator
    {
        public static async Task ValidateSocialPlatformsAsync()
        {
            using DatabaseContext dbContext = new DatabaseContext();
            List<SocialPlatform> socialPlatforms = await dbContext.SocialPlatforms.ToListAsync();
            if (socialPlatforms.Count == 0)
            {
                List<string> platforms = new List<string>
                {
                    "twitter",
                    "facebook",
                    "twitch",
                    "instagram",
                    "youtube"
                };

                foreach (string platform in platforms)
                {
                    dbContext.Add(new SocialPlatform
                    {
                        Slug = platform,
                        Name = StringExtensions.FirstCharToUpper(platform),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await dbContext.SaveChangesAsync();
            }
            return;
        }

        public static List<OldModel.Users> GetValidUsersAsync(List<OldModel.Users> users)
        {
            List<OldModel.Users> tUsers = new List<OldModel.Users>();
            foreach (OldModel.Users user in users)
            {
                if (tUsers.Find(u => u.email == user.email) == null)
                {
                    tUsers.Add(user);
                }
            }
            return tUsers;
        }
    }
}
