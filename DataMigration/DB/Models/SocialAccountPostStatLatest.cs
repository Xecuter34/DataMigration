using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPostStatLatest
    {
        public int Id { get; set; }
        public int SocialAccountStatMetricId { get; set; }
        public int SocialAccountPostId { get; set; }
        public string value { get; set; }
    }
}
