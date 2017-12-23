using System.Collections.Generic;
using Newtonsoft.Json;

namespace NerdBotScryFallPlugin
{
    public class ScryFallCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("multiverse_ids")]
        public List<int> MultiverseIds { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image_uris")]
        public Dictionary<string, string> Image_Uris { get; set; }

        [JsonProperty("usd")]
        public string PriceUsd { get; set; }

        [JsonProperty("purchase_uris")]
        public Dictionary<string, string> Purchase_Uris { get; set; }
    }
}