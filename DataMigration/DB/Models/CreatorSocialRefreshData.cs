using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class CreatorSocialRefreshData
    {
        public int Id { get; set; }
        public int CreatorSocialRefreshId { get; set; }
        public int StatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
    }
}
