using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatMetrics
    {
        public int Id { get; set; }
        public int SocialPlatformId { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }
}
