using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NerdBotCommon.Autocomplete;
using NerdBotCommon.Http;
using Newtonsoft.Json;
using SimpleLogging.Core;

namespace NerdBotCommon.ThirdParty.ScryFall
{
    public class ScryFallAutocomplete : IAutocompleter
    {
        private readonly IHttpClient mHttpClient;
        private readonly ILoggingService mLoggingService;

        private const string cUrl = "https://api.scryfall.com/cards/autocomplete?q={0}";

        public ScryFallAutocomplete(IHttpClient httpClient, ILoggingService loggingService)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mHttpClient = httpClient;
            this.mLoggingService = loggingService;
        }

        public async Task<List<string>> GetAutocompleteAsync(string term)
        {
            string json = await this.mHttpClient.GetAsJson(string.Format(cUrl, term));

            if (string.IsNullOrEmpty(json))
                return null;

            var def = JsonConvert.DeserializeObject<ScryFallCatalog>(json);

            if (def == null)
                return null;

            return def.Data;
        }
    }
}