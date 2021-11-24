using DataMigration.OldModel.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.OldModel
{
    class Posts
    {
        public ObjectId _id { get; set; }
        public string platform { get; set; }
        public string platformPostId { get; set; }
        public string accountId { get; set; }
        public Date createdAt { get; set; }
        public string imageUrl { get; set; }
        public string primaryText { get; set; }
        public string rawData { get; set; }
        public string secondaryText { get; set; }
        public Date updatedAt { get; set; }
        public string[] tags { get; set; }
    }
}
