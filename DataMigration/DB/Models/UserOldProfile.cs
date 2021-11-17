using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class UserOldProfile
    {
        public int Id { get; set; }
        public Guid NewUserId { get; set; }
        public string OldUserId { get; set; }
    }
}
