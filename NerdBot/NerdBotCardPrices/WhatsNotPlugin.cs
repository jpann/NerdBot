using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NerdBotCardPrices
{
    public class WhatsNotPlugin : PluginBase
    {
        public override string Name
        {
            get { return "whatsnot Command"; }
        }

        public override string Description
        {
            get { return "Returns the cards that had their price decreased the most.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns the cards that had their price decreased the most."; }
        }

        public override string Command
        {
            get { return "whatsnot"; }
        }

        public override string HelpCommand
        {
            get { return "help whatsnot"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: '{0}", this.Command); }
        }

        public WhatsNotPlugin(
                IBotServices services,
                BotConfig config
            )
            : base(
                services,
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

            List <CardPrice> hotCards = this.Services.PriceStore.GetCardsByPriceDecrease(5);

            if (hotCards != null)
            {
                if (hotCards.Any())
                {
                    string msg = "Today's Not-So-Hot Cards - ";

                    msg += string.Join(", ",
                        hotCards.Select(c => 
                            string.Format("{0} [{1}]: {2} down {3}", 
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
