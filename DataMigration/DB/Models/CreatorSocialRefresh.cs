using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class CreatorSocialRefresh
    {
        public int Id { get; set; }
        public Guid CreatorSocialAccountId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Status { get; set; }
        public int NumOfApiCalls { get; set; }
    }
}
