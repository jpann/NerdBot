using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using Newtonsoft.Json;

namespace NerdBotTappedOut.Fetchers
{
    public class TappedOutLatestDeckData
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Slug { get; set; }
        public string Resource_Url { get; set; }
    }

    public class TappedOutLatestFetcher
    {
        private readonly IHttpClient mHttpClient;
        private string mUrl;

        public TappedOutLatestFetcher(string url, IHttpClient httpClient)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            this.mHttpClient = httpClient;
            this.mUrl = url;
        }

        public async Task<List<TappedOutLatestDeckData>> GetLatest()
        {
            try
            {
                string latestJson = await this.mHttpClient.GetAsJson(this.mUrl);

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                var latest = JsonConvert.DeserializeObject<List<TappedOutLatestDeckData>>(latestJson);

                if (latest == null)
                    return null;

                return latest;
            }
            catch (Exception er)
            {
                Console.WriteLine("ERROR getting latest EDH: {0}", er.Message);

                throw;
            }
        }
    }
}
