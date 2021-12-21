using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatLatest
    {
        public int Id { get; set; }
        public Guid CreatorSocialAccountId { get; set; }
        public int SocialAccountStatsMetricId { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
