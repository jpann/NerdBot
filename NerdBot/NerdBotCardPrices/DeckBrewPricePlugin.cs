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
            get { return string.Format("{0} example usage: 'tcg spore clou%' or 'tcg mm2;noble%'", this.Command); }
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
                    var tcgPrice = this.mPriceStore.GetCardPrice(card.Name, card.SetId);

                    // If price is null, check again without using set code
                    if (tcgPrice == null)
                    {
                        tcgPrice = this.mPriceStore.GetCardPrice(card.Name);
                    }

                    if (tcgPrice != null)
                    {
                        if (string.IsNullOrEmpty(tcgPrice.PriceFoil) && 
                            string.IsNullOrEmpty(tcgPrice.PriceLow) &&
                            string.IsNullOrEmpty(tcgPrice.PriceMid))
                        {
                            messenger.SendMessage("Price unavailable");
                            return true;
                        }

                        string msg =
                            string.Format(
                                "{0} [{1}] {2}{3}{4}. 7-Day change: {5}.",
                                tcgPrice.Name,
                                tcgPrice.SetCode,
                                !string.IsNullOrEmpty(tcgPrice.PriceLow) ? "Low: " + tcgPrice.PriceLow + "; " : "",
                                !string.IsNullOrEmpty(tcgPrice.PriceMid) ? "Mid: " + tcgPrice.PriceMid + "; " : "",
                                !string.IsNullOrEmpty(tcgPrice.PriceFoil) ? "Foil: " + tcgPrice.PriceFoil : "",
                                !string.IsNullOrEmpty(tcgPrice.PriceDiff) ? tcgPrice.PriceDiff : "0%");

                        // Get other sets card is in
                        List<Set> otherSets = await base.Store.GetCardOtherSets(card.MultiverseId);
                        if (otherSets.Any())
                        {
                            msg += string.Format(". Also appears in sets: {0}",
                                string.Join(", ", otherSets.Select(s => s.Code).Take(10).ToArray()));
                        }

                        messenger.SendMessage(msg);

                        return true;
                    }
                    else
                    {
                        messenger.SendMessage("Price unavailable");

                        return true;
                    }
                }
                else
                {
                    this.mLoggingService.Warning("Couldn't find card using arguments.");

                    // Try a second time, this time adding in wildcards
                    string name = command.Arguments[0];

                    name = name.Replace(" ", "%");

                    card = await this.Store.GetCard(name);
                    if (card != null)
                    {
                        LoggingService.Trace("Second try using '{0}' returned a card. Suggesting '{0}'...", name, card.Name);

                        string msg = string.Format("Did you mean '{0}'?", card.Name);

                        messenger.SendMessage(msg);
                    }
                }
            }

            return false;
        }
    }
}
