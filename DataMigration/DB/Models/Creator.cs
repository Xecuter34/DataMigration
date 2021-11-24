using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class Creator
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int AddressId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsPrivate { get; set; }
    }
}
