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
using NerdBot.Plugin;
using NerdBot.UrlShortners;
using NerdBotCardPrices.PriceFetchers;

namespace NerdBotCardPrices
{
    public class MarketPricePlugin : PluginBase
    {
        private const string cUrl = "https://api.deckbrew.com/mtg";

        public override string Name
        {
            get { return "price Command"; }
        }

        public override string Description
        {
            get { return "Returns a card's market price from EchoMtg";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a card's market price from EchoMtg"; }
        }

        public override string Command
        {
            get { return "price"; }
        }

        public override string HelpCommand
        {
            get { return "help price"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'price spore clou%' or 'price mm2;noble%'", this.Command); }
        }

        public MarketPricePlugin(
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
                    string name = command.Arguments[1];
                    string set = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    // Get card using only name
                    card = await this.mStore.GetCard(name, set);
                }

                if (card != null)
                {
                    var priceData = this.mPriceStore.GetCardPrice(card.Name, card.SetId);

                    if (priceData != null)
                    {
                        if (string.IsNullOrEmpty(priceData.PriceFoil) &&
                            string.IsNullOrEmpty(priceData.PriceLow) &&
                            string.IsNullOrEmpty(priceData.PriceMid))
                        {
                            messenger.SendMessage("Price unavailable");
                            return true;
                        }

                        string msg =
                            string.Format(
                                "{0} [{1}] {2}{3}{4}. 7-Day change: {5}.",
                                priceData.Name,
                                priceData.SetCode,
                                !string.IsNullOrEmpty(priceData.PriceLow) ? "Low: " + priceData.PriceLow + "; " : "",
                                !string.IsNullOrEmpty(priceData.PriceMid) ? "Mid: " + priceData.PriceMid + "; " : "",
                                !string.IsNullOrEmpty(priceData.PriceFoil) ? "Foil: " + priceData.PriceFoil : "",
                                !string.IsNullOrEmpty(priceData.PriceDiff) ? priceData.PriceDiff : "0%");
                        
                        // Get other sets card is in
                        List<Set> otherSets = await base.Store.GetCardOtherSets(card.MultiverseId);
                        if (otherSets.Any())
                        {
                            msg += string.Format(" Also appears in sets: {0}",
                                string.Join(", ", otherSets.Select(s => s.Code).Take(5).ToArray()));
                        }

                        messenger.SendMessage(msg);

                        return true;
                    }
                    else
                    {
                        messenger.SendMessage("Market price unavailable.");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
