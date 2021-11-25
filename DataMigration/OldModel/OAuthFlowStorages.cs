using DataMigration.OldModel.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.OldModel
{
    class OAuthFlowStorages
    {
        public ObjectId _id { get; set; }
        public string[] userIdentifiers { get; set; }
        public int platform { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public Date expiry { get; set; }
        public string platformUserId { get; set; }
        public int __v { get; set; }
    }
}
