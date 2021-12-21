using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPostStatLatest
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SocialAccountPostStatsMetricId { get; set; }
        public Guid SocialAccountPostId { get; set; }
    }
}
