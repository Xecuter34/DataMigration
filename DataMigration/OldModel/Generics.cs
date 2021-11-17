using System.Text.Json.Serialization;

namespace DataMigration.OldModel.Generics
{
    class ObjectId
    {
        [JsonPropertyName("$oid")]
        public string oid { get; set; }
    }

    class DateDynamic
    {
        [JsonPropertyName("$date")]
        public dynamic date { get; set; }
    }

    class Date
    {
        [JsonPropertyName("$date")]
        public string date { get; set; }
    }
}
