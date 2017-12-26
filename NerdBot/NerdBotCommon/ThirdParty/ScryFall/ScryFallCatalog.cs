
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NerdBotCommon.ThirdParty.ScryFall
{
    public class ScryFallCatalog
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("total_values")]
        public int TotalValues { get; set; }

        [JsonProperty("data")]
        public List<string> Data { get; set; }

        public ScryFallCatalog()
        {
            this.Object = string.Empty;
            this.Uri = string.Empty;
            this.TotalValues = 0;
            this.Data = new List<string>();
        }
    }
}
