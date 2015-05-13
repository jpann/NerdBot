using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.UrlShortners;

namespace NerdBotCardPrices
{
    public class WhatsHotPlugin : PluginBase
    {
        public override string Name
        {
            get { return "whatshot Command"; }
        }

        public override string Description
        {
            get { return "Returns the cards that had their price increase the most.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns the cards that had their price increase the most.";  }
        }

        public override string Command
        {
            get { return "whatshot"; }
        }

        public override string HelpCommand
        {
            get { return "help whatshot"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: '{0}", this.Command); }
        }

        public WhatsHotPlugin(
                IMtgStore store,
                ICardPriceStore priceStore,
                ICommandParser commandParser,
                IHttpClient httpClient,
                IUrlShortener urlShortener)
            : base(
                store,
                priceStore,
                commandParser,
                httpClient,
                urlShortener)
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

            List <CardPrice> hotCards = this.PriceStore.GetCardsByPriceIncrease(5);

            if (hotCards != null)
            {
                if (hotCards.Any())
                {
                    string msg = "Today's Hot Cards - ";

                    msg += string.Join(", ",
                        hotCards.Select(c => 
                            string.Format("{0} [{1}]: {2} up {3}", 
                                c.Name, 
                                c.SetCode, 
                                c.PriceMid,
                                c.PriceDiff)).ToArray());

                    messenger.SendMessage(msg);

                    return true;
                }
            }

            return false;
        }
    }
}
