﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataMigration.DB.Models
{
    public class CreatorSocialAccount
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public int SocialPlantformId { get; set; }
        // public string SocialPlatformUserId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
