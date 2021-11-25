using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class AccountsOldProfile
    {
        public int Id { get; set; }
        public Guid NewAccountId { get; set; }
        public string OldAccountId { get; set; }
    }
}
