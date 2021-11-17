using System;
using System.ComponentModel.DataAnnotations;

namespace DataMigration.DB.Models
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public bool TermsSigned { get; set; }
        public int? SignupPreferenceId { get; set; }
        public DateTime? VerifiedAt { get; set; }
        [Required]
        public Guid ValidationToken { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? AddressId { get; set; }
    }
}
