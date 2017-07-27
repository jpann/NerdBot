using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace NerdBot.Mtg
{
    public class Card
    {
        public Card()
        {
            this.Rulings = new List<Ruling>();
            this.Colors = new List<string>();
        }

        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [JsonProperty("relatedCardId")]
        [BsonElement("relatedCardId")]
        public int? RelatedCardId { get; set; }

        [JsonProperty("name")]
        [BsonElement("name")]
        public string Name { get; set; }

        [JsonProperty("searchName")]
        [BsonElement("searchName")]
        public string SearchName { get; set; }

        [JsonProperty("desc")]
        [BsonElement("desc")]
        public string Desc { get; set; }

        [JsonProperty("flavor")]
        [BsonElement("flavor")]
        public string Flavor { get; set; }

        [JsonProperty("colors")]
        [BsonElement("colors")]
        public List<string> Colors { get; set; }

        [JsonProperty("cost")]
        [BsonElement("cost")]
        public string Cost { get; set; }

        [JsonProperty("cmc")]
        [BsonElement("cmc")]
        public double Cmc { get; set; }

        [JsonProperty("setName")]
        [BsonElement("setName")]
        public string SetName { get; set; }

        [JsonProperty("setSearchName")]
        [BsonElement("setSearchName")]
        public string SetSearchName { get; set; }

        [JsonProperty("type")]
        [BsonElement("type")]
        public string Type { get; set; }

        [JsonProperty("subType")]
        [BsonElement("subType")]
        public string SubType { get; set; }

        [JsonProperty("power")]
        [BsonElement("power")]
        public string Power { get; set; }

        [JsonProperty("toughness")]
        [BsonElement("toughness")]
        public string Toughness { get; set; }

        [JsonProperty("loyalty")]
        [BsonElement("loyalty")]
        public string Loyalty { get; set; }

        [JsonProperty("rarity")]
        [BsonElement("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("artist")]
        [BsonElement("artist")]
        public string Artist { get; set; }

        [JsonProperty("setId")]
        [BsonElement("setId")]
        public string SetId { get; set; }

        [JsonProperty("token")]
        [BsonElement("token")]
        public bool? Token { get; set; }

        [JsonProperty("rulings")]
        [BsonElement("rulings")]
        public List<Ruling> Rulings { get; set; }

        [JsonProperty("img")]
        [BsonElement("img")]
        public string Img { get; set; }

        [JsonProperty("imgHires")]
        [BsonElement("imgHires")]
        public string ImgHires { get; set; }

        [JsonProperty("multiverseId")]
        [BsonElement("multiverseId")]
        public int MultiverseId { get; set; }
    }

    public class Ruling
    {
        [BsonElement("releasedOn")]
        [JsonProperty("releasedOn")]
        public DateTime ReleasedOn { get; set; }

        [BsonElement("rule")]
        [JsonProperty("rule")]
        public string Rule { get; set; }
    }
}
