using System;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace NerdBot.Mtg
{
    public class Set
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [JsonProperty("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonProperty("searchName")]
        [BsonElement("searchName")]
        public string SearchName { get; set; }

        [JsonProperty("code")]
        [BsonElement("code")]
        public string Code { get; set; }

        [JsonProperty("block")]
        [BsonElement("block")]
        public string Block { get; set; }

        [JsonProperty("type")]
        [BsonElement("type")]
        public string Type { get; set; }

        [JsonProperty("desc")]
        [BsonElement("desc")]
        public string Desc { get; set; }

        [JsonProperty("commonQty")]
        [BsonElement("commonQty")]
        public int CommonQty { get; set; }

        [JsonProperty("uncommonQty")]
        [BsonElement("uncommonQty")]
        public int UncommonQty { get; set; }

        [JsonProperty("rareQty")]
        [BsonElement("rareQty")]
        public int RareQty { get; set; }

        [JsonProperty("mythicQty")]
        [BsonElement("mythicQty")]
        public int MythicQty { get; set; }

        [JsonProperty("basicLandQty")]
        [BsonElement("basicLandQty")]
        public int BasicLandQty { get; set; }

        [JsonProperty("totalQty")]
        [BsonElement("totalQty")]
        public int TotalQty
        {
            get
            {
                return this.CommonQty + this.UncommonQty + this.RareQty + this.MythicQty + this.BasicLandQty;
            }
        }

        [JsonProperty("releasedOn")]
        [BsonElement("releasedOn")]
        public DateTime ReleasedOn { get; set; }

        [JsonProperty("gathererCode")]
        [BsonElement("gathererCode")]
        public string GathererCode { get; set; }

        [JsonProperty("oldCode")]
        [BsonElement("oldCode")]
        public string OldCode { get; set; }

        [JsonProperty("magicCardsInfoCode")]
        [BsonElement("magicCardsInfoCode")]
        public string MagicCardsInfoCode { get; set; }
    }
}
