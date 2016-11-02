using System;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.UrlShortners;

namespace NerdBotCoreCommands
{
    public class RandomFlavorTextPlugin: PluginBase
    {
        public override string Name
        {
            get { return "flavor command"; }
        }

        public override string Description
        {
            get { return "Returns a random card's flavor text.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card's flavor text."; }
        }

        public override string Command
        {
            get { return "flavor"; }
        }

        public override string HelpCommand
        {
            get { return "help flavor"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'flavor'", this.Command); }
        }

        public RandomFlavorTextPlugin(
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

            string flavor = await base.Store.GetRandomFlavorText();

            if (!string.IsNullOrEmpty(flavor))
            {
                messenger.SendMessage(flavor);

                return true;
            }

            return false;
        }
    }
}
