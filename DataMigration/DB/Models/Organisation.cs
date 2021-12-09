using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class Organisation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? AddressId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
