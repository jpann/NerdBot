using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace NerdBot.Statistics
{
    public class CardQueryStatData
    {
        public string UserName { get; set; }
        public int MultiverseId { get; set; }
        public int Count { get; set; }
    }

    public class CardQueryStat
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [JsonProperty("userName")]
        [BsonElement("userName")]
        public string UserName { get; set; }

        [JsonProperty("userId")]
        [BsonElement("userId")]
        public int UserId { get; set; }

        [JsonProperty("dateTime")]
        [BsonElement("dateTime")]
        public DateTime Date { get; set; }

        [JsonProperty("multiverseId")]
        [BsonElement("multiverseId")]
        public int MultiverseId { get; set; }
    }
}
