using DataMigration.OldModel.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.OldModel
{
    class DetailedPostsCluster
    {
        public ObjectId _id { get; set; }
        public Date endDate { get; set; }
        public Date startDate { get; set; }
        public ObjectId postId { get; set; }
        public Slices[] slices { get; set; }
        public int __v { get; set; }
    }
}
