using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class Creator
    {
        public Guid Id { get; set; }
       // public Guid CreatorId { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
