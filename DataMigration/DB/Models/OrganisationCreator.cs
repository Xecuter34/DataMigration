using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class OrganisationCreator
    {
        public int Id { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
