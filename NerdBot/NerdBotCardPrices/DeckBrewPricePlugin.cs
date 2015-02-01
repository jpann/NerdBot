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
    public class DeckBrewPricePlugin : PluginBase
    {
        private const string cUrl = "https://api.deckbrew.com/mtg";

        public override string Name
        {
            get { return "tcg Command"; }
        }

        public override string Description
        {
            get { return "Returns a card's TCGPlayer price using DeckBrew";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a card's TCGPlayer price using DeckBrew"; }
        }

        public override string Command
        {
            get { return "tcg"; }
        }

        public override string HelpCommand
        {
            get { return "help tcg"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'tcg spore clou%'", this.Command); }
        }

        public DeckBrewPricePlugin(
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
                    var tcg = new DeckBrewPriceFetcher(cUrl, base.HttpClient);
                    string[] tcgPrice = tcg.GetPrice(card.MultiverseId);

                    if (tcgPrice != null)
                    {
                        // Not all cards have a price from DeckBrew, so check for 0 prices here,
                        //  to prevent posting of 0.00 prices
                        if (tcgPrice[2] == "0.00" && tcgPrice[1] == "0.00" && tcgPrice[0] == "0.00")
                        {
                            messenger.SendMessage("Price unavailable using DeckBrew");
                            return true;
                        }

                        string url = this.UrlShortener.ShortenUrl(tcgPrice[3]);

                        string msg = "";

                        if (!string.IsNullOrEmpty(url))
                        {
                            msg = string.Format("{0} [{1}] - High: {2}; Median: {3}; Low: {4} - {5}",
                                card.Name,
                                card.SetName,
                                tcgPrice[2],
                                tcgPrice[1],
                                tcgPrice[0],
                                url);
                        }
                        else
                        {
                            msg = string.Format("{0} [{1}] - High: {2}; Median: {3}; Low: {4}",
                                card.Name,
                                card.SetName,
                                tcgPrice[2],
                                tcgPrice[1],
                                tcgPrice[0]);
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
                        messenger.SendMessage("Price unavailable using DeckBrew");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
