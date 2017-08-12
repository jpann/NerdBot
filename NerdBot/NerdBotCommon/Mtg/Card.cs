using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using NerdBotCommon.Utilities;
using Newtonsoft.Json;

namespace NerdBotCommon.Mtg
{
    public class Card
    {
        public Card()
        {
            this.Rulings = new List<Ruling>();
            this.Colors = new List<string>();
            this.ColorIdentity = new List<string>();
            this.Variations = new List<int?>();
            this.Types = new List<string>();
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

        public string DescWithSymbols
        {
            get { return SymbolsUtility.GetDescSymbols(this.Desc); }
        }

        [JsonProperty("flavor")]
        [BsonElement("flavor")]
        public string Flavor { get; set; }

        [JsonProperty("colors")]
        [BsonElement("colors")]
        public List<string> Colors { get; set; }

        [JsonProperty("cost")]
        [BsonElement("cost")]
        public string Cost { get; set; }

        public string CostWithSymbols
        {
            get { return SymbolsUtility.GetCostSymbols(this.Cost); }
        }

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

        public string SetAsKeyRuneIcon
        {
            get
            {
                string set = "ss ss-{0} ss-{1}";

                switch (this.Rarity)
                {
                    case "Basic Land":
                        set = string.Format(set, this.SetId.ToLower(), "common");
                        break;
                    case "Common":
                        set = string.Format(set, this.SetId.ToLower(), "common");
                        break;
                    case "Uncommon":
                        set = string.Format(set, this.SetId.ToLower(), "uncommon");
                        break;
                    case "Rare":
                        set = string.Format(set, this.SetId.ToLower(), "rare");
                        break;
                    case "Mythic Rare":
                        set = string.Format(set, this.SetId.ToLower(), "mythic");
                        break;
                    case "Special":
                        set = string.Format(set, this.SetId.ToLower(), "timeshifted");
                        break;
                    default:
                        set = string.Format(set, this.SetId.ToLower(), "common");
                        break;
                }

                return set;
            }
        }

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

        public string FullType
        {
            get
            {
                return !string.IsNullOrEmpty(this.SubType) ? string.Format("{0} - {1}", this.Type, this.SubType) : this.Type;
            }
        }

        [JsonProperty("colorIdentity")]
        [BsonElement("colorIdentity")]
        public List<string> ColorIdentity { get; set; }

        [JsonProperty("types")]
        [BsonElement("types")]
        public List<string> Types { get; set; }

        [JsonProperty("number")]
        [BsonElement("number")]
        public string Number { get; set; }

        [JsonProperty("variations")]
        [BsonElement("variations")]
        public List<int?> Variations { get; set; }

        [JsonProperty("mciNumber")]
        [BsonElement("mciNumber")]
        public string McINumber { get; set; }
    }

    public class Ruling
    {
        [BsonElement("releasedOn")]
        [JsonProperty("releasedOn")]
        public DateTime ReleasedOn { get; set; }

        public string ReleasedOnShort
        {
            get { return this.ReleasedOn.ToShortDateString(); }
        }

        [BsonElement("rule")]
        [JsonProperty("rule")]
        public string Rule { get; set; }

        public string RuleWithSymbols
        {
            get { return SymbolsUtility.GetDescSymbols(this.Rule); }
        }
    }
}
