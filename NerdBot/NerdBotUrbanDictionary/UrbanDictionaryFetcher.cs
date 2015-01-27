using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using Newtonsoft.Json;

namespace NerdBotUrbanDictionary
{
    public class UrbanDictionaryData
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("list")]
        public List<UrbanDictionaryDefinition> Definitions { get; set; } 
    }

    public class UrbanDictionaryDefinition
    {
        [JsonProperty("defid")]
        public int DefId { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("permalink")]
        public string PermaLink { get; set; }

        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }
    }

    public class UrbanDictionaryFetcher
    {
        private readonly IHttpClient mHttpClient;
        private readonly string mUrl;

        public UrbanDictionaryFetcher(string url, IHttpClient httpClient)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mUrl = url;
            this.mHttpClient = httpClient;
        }

        public async Task<UrbanDictionaryData> GetDefinition(string word)
        {
            try
            {
                //TODO Sometimes api.urbandictionary.com returns a 503 status...
                string latestJson = await this.mHttpClient.GetAsJson(string.Format(this.mUrl, word));

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                var def = JsonConvert.DeserializeObject<UrbanDictionaryData>(latestJson);

                if (def == null)
                    return null;

                return def;
            }
            catch (Exception er)
            {
                string msg = string.Format("ERROR getting urban dictionary definition for '{0}': {1}",
                    word,
                    er.Message);

                Console.WriteLine(msg);

                throw;
            }
        }
    }
}
