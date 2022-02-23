using DataMigration.OldModel.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataMigration.OldModel
{
    class Accounts
    {
        public ObjectId _id { get; set; }
        public bool backfilled { get; set; }
        public int collectRate { get; set; }
        public Date updatedAt { get; set; }
        public Date connectedOn { get; set; }
        public bool archived { get; set; }
        public string platform { get; set; }
        public UniqueIdentifiers uniqueIdentifiers { get; set; }
        public List<ExternalSourceRelations> externalSourceRelations { get; set; }
        public Date nextUpdateDue { get; set; }
        public int __v { get; set; }
        public Meta meta { get; set; }
    }

    class UniqueIdentifiers
    {
        public string oauthLookupId { get; set; }
        public string platformUserId { get; set; }
        public string pageId { get; set; }
        public string igId { get; set; }
        public string channelId { get; set; }
    }

    class ExternalSourceRelations
    {
        public ObjectId _id { get; set; }
        public int source { get; set; }
        public string sourceId { get; set; }
    }

    class Meta
    {
        public string name { get; set; }
        public string avatar { get; set; }
    }
}
