using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class SocialAccountPost
    {
        public Guid Id { get; set; }
        public Guid CreatorSocialAccountId { get; set; }
        public string SocialMedialUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
