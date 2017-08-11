using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Nancy.Json;
using Newtonsoft.Json;

namespace NerdBotCommon.Mtg.Prices
{
    public class SetPrice
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        [ScriptIgnore]
        public ObjectId Id { get; set; }

        [JsonProperty("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonProperty("searchName")]
        [BsonElement("searchName")]
        public string SearchName { get; set; }

        [JsonProperty("setCode")]
        [BsonElement("setCode")]
        public string SetCode { get; set; }

        [JsonProperty("totalCards")]
        [BsonElement("totalCards")]
        public int TotalCards { get; set; }

        [JsonProperty("setValue")]
        [BsonElement("setValue")]
        public string SetValue { get; set; }

        [JsonProperty("foilSetValue")]
        [BsonElement("foilSetValue")]
        public string FoilSetValue { get; set; }

        [JsonProperty("url")]
        [BsonElement("url")]
        public string Url { get; set; }

        [JsonProperty("lastUpdated")]
        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
}
