using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class OrganisationUser
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
        public bool IsOwner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
