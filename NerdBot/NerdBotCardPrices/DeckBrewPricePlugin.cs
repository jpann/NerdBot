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

        public DeckBrewPricePlugin(
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
            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            var command = this.mCommandParser.Parse(message.text);

            // If there was no command, return
            if (command == null)
                return false;

            // Parse command
            if (command.Cmd.ToLower() == "tcg")
            {
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
                        }
                        else
                        {
                            messenger.SendMessage("Price unavailable");
                        }
                    }
                }
            }

            return false;
        }
    }
}
