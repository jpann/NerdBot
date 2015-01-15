using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.UrlShortners;
using NerdBotCardPrices.PriceFetchers;

namespace NerdBotCardPrices
{
    public class EbayPricePlugin : PluginBase
    {
        public override string Name
        {
            get { return "ebay Command"; }
        }

        public override string Description
        {
            get { return "Returns a card's eBay buy it now price.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a card's eBay buy it now price."; }
        }

        public override string Command
        {
            get { return "ebay"; }
        }

        public override string HelpCommand
        {
            get { return "help ebay"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0}: HELP TEXT HERE", this.Command); }
        }

        public EbayPricePlugin(
                IMtgStore store,
                ICommandParser commandParser,
                IHttpClient httpClient,
                IUrlShortener urlShortener)
            : base(
                store,
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

            if (command.Arguments.Any())
            {
                Card card = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    // Get card using only name
                    card = await this.mStore.GetCard(name);
                }
                else if (command.Arguments.Length == 2)
                {
                    string name = command.Arguments[0];
                    string set = command.Arguments[1];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    // Get card using only name
                    card = await this.mStore.GetCard(name, set);
                }

                if (card != null)
                {
                    var ebay = new EbayPriceFetcher();
                    string[] ebayPrice = ebay.GetPrice(card.Name);

                    if (ebayPrice != null)
                    {
                        string url = this.UrlShortener.ShortenUrl(ebayPrice[1]);

                        string msg = "";

                        if (!string.IsNullOrEmpty(url))
                        {
                            msg = string.Format("The eBay Buy It Now price for '{0}' is {1} - {2}",
                                card.Name,
                                ebayPrice[0],
                                url);
                        }
                        else
                        {
                            msg = string.Format("The eBay Buy It Now price for '{0}' is {1}",
                                card.Name,
                                ebayPrice[0]);
                        }

                        // Get other sets card is in
                        List<Set> otherSets = await base.Store.GetCardOtherSets(card.MultiverseId);
                        if (otherSets.Any())
                        {
                            msg += string.Format(". Also appears in sets: {0}",
                                string.Join(", ", otherSets.Select(s => s.Name).Take(5).ToArray()));
                        }

                        messenger.SendMessage(msg);

                        return true;
                    }
                    else
                    {
                        messenger.SendMessage("Price unavailable");
                    }
                }
            }

            return false;
        }
    }
}
