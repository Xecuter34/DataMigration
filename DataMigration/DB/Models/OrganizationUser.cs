using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class OrganizationUser
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
