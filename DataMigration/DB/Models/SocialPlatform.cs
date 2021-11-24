using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialPlatform
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public int CollectRateInMinutes { get; set; }
    }
}
