using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatHistory
    {
        public int Id { get; set; }
        public int SocialAccountStatMetricId { get; set; }
        public int CreatorSocialRefreshId { get; set; }
        public DateTime CollectedAt { get; set; }
        public string Value { get; set; }
    }
}
