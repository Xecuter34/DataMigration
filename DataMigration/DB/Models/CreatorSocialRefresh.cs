using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class CreatorSocialRefresh
    {
        public int Id { get; set; }
        public Guid CreatorSocialAccountId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public int NumApiCalls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SocialAccountRefreshStatusId { get; set; }
    }
}
