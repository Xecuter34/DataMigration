using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountStatMetrics
    {
        public int Id { get; set; }
        public int SocialPlatformId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
    }
}
