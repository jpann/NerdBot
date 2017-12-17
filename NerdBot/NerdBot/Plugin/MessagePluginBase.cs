
using System;
using System.Threading.Tasks;
using NerdBot.Parsers;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.UrlShortners;
using SimpleLogging.Core;

namespace NerdBot.Plugin
{
    public abstract class MessagePluginBase : IMessagePlugin
    {
        protected string mBotName;
        protected IBotServices mServices;
        protected ILoggingService mLoggingService;

        public MessagePluginBase(
            IBotServices services)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            this.mServices = services;
        }

        #region Properties
        public string BotName
        {
            set
            {
                this.mBotName = value;
            }

            get { return this.mBotName; }
        }

        public IBotServices Services
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.mServices = value;
            }
            get { return this.mServices; }
        }

        public ILoggingService LoggingService
        {
            get { return this.mLoggingService; }
            set { this.mLoggingService = value; }
        }

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ShortDescription { get; }
        #endregion

        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}
