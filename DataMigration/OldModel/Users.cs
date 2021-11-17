using System;
using DataMigration.OldModel.Generics;

namespace DataMigration.OldModel
{
    class Users
    {
        public ObjectId _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateDynamic dob { get; set; }
        public object address { get; set; }
        public bool termsSigned { get; set; }
        public string signupPreference { get; set; }
        public Date verifiedAt { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public Guid validationToken { get; set; }
        public Date createdAt { get; set; }
        public Date updatedAt { get; set; }
        public int __v { get; set; }
        public string password { get; set; }
        public dynamic enabledFeatures { get; set; }
        public string[] tours { get; set; }
    }
}
