using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace NerdBotCommon.Statistics
{
    public class CardQueryStat
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [JsonProperty("userName")]
        [BsonElement("userName")]
        public string UserName { get; set; }

        [JsonProperty("userId")]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [JsonProperty("dateTime")]
        [BsonElement("dateTime")]
        public DateTime Date { get; set; }

        [JsonProperty("multiverseId")]
        [BsonElement("multiverseId")]
        public int MultiverseId { get; set; }

        [JsonProperty("searchTerm")]
        [BsonElement("searchTerm")]
        public string SearchTerm { get; set; }

        [BsonIgnoreIfNull]
        public double? TextMatchScore { get; set; }
    }

    public class QueryStat
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [JsonProperty("dateTime")]
        [BsonElement("dateTime")]
        public DateTime Date { get; set; }

        [JsonProperty("searchTerm")]
        [BsonElement("searchTerm")]
        public string SearchTerm { get; set; }

        [JsonProperty("multiverseId")]
        [BsonElement("multiverseId")]
        public int MultiverseId { get; set; }

        [BsonIgnoreIfNull]
        public double? TextMatchScore { get; set; }
    }
}
