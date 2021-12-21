using DataMigration.OldModel.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.OldModel
{
    class TrackedSocials
    {
        public ObjectId _id { get; set; }
        public Date createdAt { get; set; }
        public Date approvedAt { get; set; }
        public string status { get; set; }
        public ObjectId connectionId { get; set; }
        public ObjectId userId { get; set; }
        public string platform { get; set; }
        public ObjectId org { get; set; }
        public ObjectId requestedBy { get; set; }
        public int __v { get; set; }
    }
}
