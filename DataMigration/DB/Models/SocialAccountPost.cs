using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPost
    {
        public Guid Id { get; set; }
        public int CreatorSocialAccountId { get; set; }
        public string SocialMediaUid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Value { get; set; }
    }
}
