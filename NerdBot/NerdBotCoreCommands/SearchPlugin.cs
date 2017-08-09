using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.UrlShortners;

namespace NerdBotCoreCommands
{
    public class SearchPlugin : PluginBase
    {
        private const string cSearchUrl = "{0}/search/{1}";

        public override string Name
        {
            get { return "search command"; }
        }

        public override string Description
        {
            get { return ""; }
        }

        public override string ShortDescription
        {
            get { return "."; }
        }

        public override string Command
        {
            get { return "search"; }
        }

        public override string HelpCommand
        {
            get { return "help search"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'search spore clou'", this.Command); }
        }

        public SearchPlugin(
                IMtgStore store,
                ICardPriceStore priceStore,
                ICommandParser commandParser,
                IHttpClient httpClient,
                IUrlShortener urlShortener,
                BotConfig config
            )
            : base(
                store,
                priceStore,
                commandParser,
                httpClient,
                urlShortener,
                config)
        {
        }

        public override void OnLoad()
        {
        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnMessage(IMessage message, IMessenger messenger)
        {
            return false;
        }

        public override async Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            if (command.Arguments.Any())
            {
                Card card = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    name = Uri.EscapeDataString(name);

                    string url = string.Format(cSearchUrl, this.Config.HostUrl, name);

                    messenger.SendMessage(string.Format("Card search: {0}", url));
                }
            }

            return false;
        }
    }
}
