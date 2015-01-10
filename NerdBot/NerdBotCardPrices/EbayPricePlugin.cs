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

        public EbayPricePlugin()
            : base()
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
            if (command.Cmd.ToLower() == "ebay")
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
                        var ebay = new EbayPriceFetcher();
                        string[] ebayPrice = ebay.GetPrice(card.Name);

                        if (ebayPrice != null)
                        {
                            messenger.SendMessage(string.Format("The eBay Buy It Now price for '{0}' is {1} - {2}",
                                card.Name,
                                ebayPrice[0],
                                ebayPrice[1]));
                        }
                    }
                }
            }

            return false;
        }
    }
}
