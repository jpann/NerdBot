using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using SimpleLogging.Core;

namespace NerdBot.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected IMtgStore mStore;
        protected ICardPriceStore mPriceStore;
        protected ICommandParser mCommandParser;
        protected IHttpClient mHttpClient;
        protected IUrlShortener mUrlShortener;
        protected ILoggingService mLoggingService;
        protected BotConfig mBotConfig;

        public PluginBase(
            IMtgStore store,
            ICardPriceStore priceStore,
            ICommandParser commandParser,
            IHttpClient httpClient,
            IUrlShortener urlShortener,
            BotConfig config)
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

            if (config == null)
                throw new ArgumentNullException("config");

            this.mStore = store;
            this.mPriceStore = priceStore;
            this.mCommandParser = commandParser;
            this.mHttpClient = httpClient;
            this.mUrlShortener = urlShortener;
            this.mBotConfig = config;
        }

        #region Properties
        public IMtgStore Store
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.mStore = value;
            }
            get { return this.mStore; }
        }

        public ICardPriceStore PriceStore
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.mPriceStore = value;
            }
            get { return this.mPriceStore; }
        }

        public ICommandParser CommandParser
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.mCommandParser = value;
            }
            get { return this.mCommandParser; }
        }

        public IHttpClient HttpClient
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.mHttpClient = value;
            }

            get { return this.mHttpClient; }
        }

        public IUrlShortener UrlShortener
        {
            set
            {
                if (value == null)
                    throw new ArgumentException("value");

                this.mUrlShortener = value;
            }

            get { return this.mUrlShortener; }
        }

        public ILoggingService LoggingService
        {
            get { return this.mLoggingService; }
            set { this.mLoggingService = value; }
        }

        public BotConfig Config { get; set; }

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ShortDescription { get; }
        public abstract string Command { get; }
        public abstract string HelpCommand { get; }
        public abstract string HelpDescription { get; }
        #endregion

        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract Task<bool> OnMessage(IMessage message, IMessenger messenger);
        public abstract Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger);
    }
}
