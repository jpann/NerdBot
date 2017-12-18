using System;
using NerdBot.Parsers;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;

namespace NerdBot.Plugin
{
    public class BotServices : IBotServices
    {
        private readonly IMtgStore mStore;
        private readonly ICardPriceStore mPriceStore;
        private readonly ICommandParser mCommandParser;
        private readonly IHttpClient mHttpClient;
        private readonly IUrlShortener mUrlShortener;
        private readonly IQueryStatisticsStore mQueryStatisticsStore;

        #region Properties
        public ICommandParser CommandParser
        {
            get { return this.mCommandParser; }
        }

        public IHttpClient HttpClient
        {
            get { return this.mHttpClient; }
        }

        public ICardPriceStore PriceStore
        {
            get { return this.mPriceStore; }
        }

        public IQueryStatisticsStore QueryStatisticsStore
        {
            get { return this.mQueryStatisticsStore; }
        }

        public IMtgStore Store
        {
            get { return this.mStore; }
        }

        public IUrlShortener UrlShortener
        {
            get { return this.mUrlShortener; }
        }
        #endregion

        public BotServices(
            IMtgStore store,
            ICardPriceStore priceStore,
            ICommandParser commandParser,
            IHttpClient httpClient,
            IUrlShortener urlShortener,
            IQueryStatisticsStore queryStatisticsStore)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            if (priceStore == null)
                throw new ArgumentNullException("priceStore");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (urlShortener == null)
                throw new ArgumentNullException("urlShortener");

            if (queryStatisticsStore == null)
                throw new ArgumentNullException("queryStatisticsStore");

            this.mStore = store;
            this.mPriceStore = priceStore;
            this.mCommandParser = commandParser;
            this.mHttpClient = httpClient;
            this.mUrlShortener = urlShortener;
            this.mQueryStatisticsStore = queryStatisticsStore;
        }
    }
}
