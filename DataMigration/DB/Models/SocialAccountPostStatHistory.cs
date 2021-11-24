using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPostStatHistory
    {
        public int Id { get; set; }
        public Guid SocialAccountPostId { get; set; }
        public int SocialAccountStatMetricId { get; set; }
        public int CreatorSocialRefreshId { get; set; }
        public DateTime CollectedAt { get; set; }
        public string value { get; set; }
    }
}
