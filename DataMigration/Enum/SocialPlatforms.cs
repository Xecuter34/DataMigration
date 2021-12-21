using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.Enum
{
    public enum SocialPlatforms
    {
        INSTAGRAM = 1,
        TWITTER = 2,
        FACEBOOK = 3,
        YOUTUBE = 4,
        TWITCH = 5
    }

    public static class SocialPlatformsExtensions
    {
        public static string GetString(this SocialPlatforms me)
        {
            return me switch
            {
                SocialPlatforms.INSTAGRAM => "71cbfdae-65aa-4061-ae0d-ae893dc297df",
                SocialPlatforms.TWITTER => "49d6caed-b979-4e6d-9e4d-338130512968",
                SocialPlatforms.FACEBOOK => "c3f960ff-f65e-457a-ac2d-f2ab2a72aea1",
                SocialPlatforms.YOUTUBE => "2249ce36-a991-47fe-95c7-bf08acef8e62",
                SocialPlatforms.TWITCH => "0417a7a8-8f1a-489e-942e-c2392a0a571e",
                _ => "NO_VALUE_GIVEN",
            };
        }

        public static int GetIntByString(string platformName)
        {
            return platformName switch
            {
                "instagram" => (int)SocialPlatforms.INSTAGRAM,
                "twitter" => (int)SocialPlatforms.TWITTER,
                "facebook" => (int)SocialPlatforms.FACEBOOK,
                "youtube" => (int)SocialPlatforms.YOUTUBE,
                "twitch" => (int)SocialPlatforms.TWITCH,
                _ => 0
            };
        }
    }
}