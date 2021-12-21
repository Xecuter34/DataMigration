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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
