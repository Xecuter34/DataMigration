using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Validated { get; set; }
        public string Logo { get; set; }
        public bool WhiteLabelEnabled { get; set; }
    }
}
