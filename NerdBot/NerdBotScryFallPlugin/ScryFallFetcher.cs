using System;
using System.Threading.Tasks;
using NerdBotCommon.Http;
using Newtonsoft.Json;

namespace NerdBotScryFallPlugin
{
    //TODO This is quick and dirty. Future plans are to cache responses for 24 hours
    public class ScryFallFetcher
    {
        private readonly IHttpClient mHttpClient;

        private const string cApiUrl = "https://api.scryfall.com";

        public ScryFallFetcher(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mHttpClient = httpClient;
        }

        public async Task<ScryFallCard> GetCard(int multiverseId)
        {
            try
            {
                string cardApi = "/cards/multiverse/{0}";

                string latestJson = await this.mHttpClient.GetAsJson(string.Format(cApiUrl + cardApi, multiverseId));

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                var def = JsonConvert.DeserializeObject<ScryFallCard>(latestJson);

                if (def == null)
                    return null;

                return def;
            }
            catch (Exception er)
            {
                string msg = string.Format("ERROR getting ScryFall Card for '{0}': {1}",
                    multiverseId,
                    er.Message);

                Console.WriteLine(msg);

                throw;
            }
        }
    }
}