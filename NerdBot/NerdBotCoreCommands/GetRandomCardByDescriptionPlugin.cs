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

namespace NerdBotCoreCommands
{
    public class GetRAndomCardByDescriptionPlugin : PluginBase
    {
        public override string Name
        {
            get { return "desc command"; }
        }

        public override string Description
        {
            get { return "Get a random card where the rules text contains the specified text."; }
        }

        public override string ShortDescription
        {
            get { return "Get a random card where the rules text contains the specified text."; }
        }

        public override string Command
        {
            get { return "desc"; }
        }

        public override string HelpCommand
        {
            get { return "help desc"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'desc flying' or 'desc greater comes into play '", this.Command); }
        }

        public GetRAndomCardByDescriptionPlugin(
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
                    string ability = command.Arguments[0];

                    if (string.IsNullOrEmpty(ability))
                        return false;

                    card = await this.Store.GetRandomCardWithDescription(ability);
                }

                if (card != null)
                {
                    messenger.SendMessage(card.Img);

                    // Get other sets card is in
                    List<Set> otherSets = await base.Store.GetCardOtherSets(card.MultiverseId);
                    if (otherSets.Any())
                    {
                        string msg = string.Format("Also in sets: {0}",
                            string.Join(", ", otherSets.Select(s => s.Code).Take(10).ToArray()));

                         messenger.SendMessage(msg);
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
