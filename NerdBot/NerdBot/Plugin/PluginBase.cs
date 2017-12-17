using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Parsers;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;
using SimpleLogging.Core;

namespace NerdBot.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected IBotServices mServices;
        protected ILoggingService mLoggingService;
        protected BotConfig mBotConfig;

        public PluginBase(
            IBotServices services,
            BotConfig config)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            if (config == null)
                throw new ArgumentNullException("config");

            this.mServices = services;
            this.mBotConfig = config;
        }

        #region Properties
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

        public BotConfig Config
        {
            get { return this.mBotConfig; }
            set { this.mBotConfig = value; }
        }

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
