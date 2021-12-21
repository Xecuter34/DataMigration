using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPostStatHistory
    {
        public int Id { get; set; }
        public int CreatorSocialRefreshId { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SocialAccountPostStatsMetricId { get; set; }
        public Guid SocialAccountPostId { get; set; }
    }
}
