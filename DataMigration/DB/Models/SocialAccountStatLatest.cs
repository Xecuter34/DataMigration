using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatLatest
    {
        public int Id { get; set; }
        public int CreatorSocialAccountId { get; set; }
        public int SocialAccountStatMetricId { get; set; }
        public int CreatorSocialRefreshId { get; set; }
    }
}
