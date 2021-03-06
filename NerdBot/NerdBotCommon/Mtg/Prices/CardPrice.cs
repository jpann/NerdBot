﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Nancy.Json;
using Newtonsoft.Json;

namespace NerdBotCommon.Mtg.Prices
{
    public class CardPrice
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

        [JsonProperty("setCodeAlternate")]
        [BsonElement("setCodeAlternate")]
        public string SetCodeAlternate { get; set; }

        public string SetAsKeyRuneIcon
        {
            get
            {
                string set = "<i class=\"ss ss-{0} ss-{1} ss-2x\"></i>";
                
                set = string.Format(set, this.SetCode.ToLower(), "common");

                return set;
            }
        }

        [JsonProperty("priceDiff")]
        [BsonElement("priceDiff")]
        public string PriceDiff { get; set; }

        [JsonProperty("priceDiffValue")]
        [BsonElement("priceDiffValue")]
        public int PriceDiffValue { get; set; }

        [JsonProperty("priceMid")]
        [BsonElement("priceMid")]
        public string PriceMid { get; set; }

        [JsonProperty("priceLow")]
        [BsonElement("priceLow")]
        public string PriceLow { get; set; }

        [JsonProperty("priceFoil")]
        [BsonElement("priceFoil")]
        public string PriceFoil { get; set; }

        [JsonProperty("url")]
        [BsonElement("url")]
        public string Url { get; set; }

        [JsonProperty("lastUpdated")]
        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("multiverseId")]
        [BsonElement("multiverseId")]
        public int MultiverseId { get; set; }

        [JsonProperty("imageUrl")]
        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }

        public CardPrice()
        {
            this.SetCodeAlternate = string.Empty;
        }
    }
}
