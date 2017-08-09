using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.UrlShortners;

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
