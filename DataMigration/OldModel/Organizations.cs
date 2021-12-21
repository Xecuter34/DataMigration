using System;
using DataMigration.OldModel.Generics;

namespace DataMigration.OldModel
{
    class Organisations
    {
        public ObjectId _id { get; set; }
        public object address { get; set; }
        public string name { get; set; }
        public ObjectId[] admins { get; set; }
        public ObjectId[] members { get; set; }
        public int __v { get; set; }
        public ObjectId[]? invited { get; set; }
        public bool? validated { get; set; }
        public string? logo { get; set; }
        public bool? whiteLabelEnabled { get; set; }
    }
}
