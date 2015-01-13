using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;

namespace NerdBot.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected IMtgStore mStore;
        protected ICommandParser mCommandParser;
        protected IHttpClient mHttpClient;
        protected IUrlShortener mUrlShortener;

        public PluginBase(
            IMtgStore store,
            ICommandParser commandParser,
            IHttpClient httpClient,
            IUrlShortener urlShortener)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (urlShortener == null)
                throw new ArgumentNullException("urlShortener");

            this.mStore = store;
            this.mCommandParser = commandParser;
            this.mHttpClient = httpClient;
            this.mUrlShortener = urlShortener;
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

        public abstract string Name { get; }
        public abstract string Description { get; }
        #endregion

        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}
