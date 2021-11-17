using System;
using System.ComponentModel.DataAnnotations;

namespace DataMigration.DB.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime Dob { get; set; }
        public string Username { get; set; }
    }
}
