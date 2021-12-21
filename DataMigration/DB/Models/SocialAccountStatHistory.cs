using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatHistory
    {
        public int Id { get; set; }
        public int CreatorSocialRefreshId { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SocialAccountStatsMetricId { get; set; }
    }
}
