using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NerdBotGiphyPlugin
{
    public class GiphyFetcher
    {
        private readonly IHttpClient mHttpClient;
        private readonly string mUrl;

        public GiphyFetcher(string url, IHttpClient httpClient)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mUrl = url;
            this.mHttpClient = httpClient;
        }

        public async Task<string> GetGiphyGif(string keyword)
        {
            try
            {
                keyword = Uri.EscapeDataString(keyword);

                string latestJson = await this.mHttpClient.GetAsJsonNonCached(string.Format(this.mUrl, keyword));

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                JObject giphy = JObject.Parse(latestJson);

				string url = (string)giphy["data"]["images"]["original"]["url"];

                return url;
            }
            catch (Exception er)
            {
                string msg = string.Format("ERROR getting giphy gif for '{0}': {1}",
                    keyword,
                    er.Message);

                Console.WriteLine(msg);

                throw;
            }
        }
    }
}
