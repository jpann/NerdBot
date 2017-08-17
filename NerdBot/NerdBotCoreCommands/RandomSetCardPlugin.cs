using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;

namespace NerdBotCoreCommands
{
    public class RandomSetCardPlugin : PluginBase
    {
        public override string Name
        {
            get { return "setrandom command"; }
        }

        public override string Description
        {
            get { return "Returns a random card from the specified set.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card from the specified set."; }
        }

        public override string Command
        {
            get { return "setrandom"; }
        }

        public override string HelpCommand
        {
            get { return "help setrandom"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'setrandom fem' or 'setrandom fifth edition'", this.Command); }
        }

        public RandomSetCardPlugin(
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

            if (command.Arguments.Length == 1)
            {
                Card card = await base.Store.GetRandomCardInSet(command.Arguments[0]);

                if (card != null)
                {
                    messenger.SendMessage(card.Img);

                    return true;
                }
            }

            return false;
        }
    }
}
