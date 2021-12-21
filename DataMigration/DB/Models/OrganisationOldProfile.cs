using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.DB.Models
{
    public class OrganisationOldProfile
    {
        public int Id { get; set; }
        public Guid NewOrgId { get; set; }
        public string OldOrgId { get; set; }
    }
}
