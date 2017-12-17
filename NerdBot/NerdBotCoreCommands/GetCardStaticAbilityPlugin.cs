using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NerdBotCoreCommands
{
    public class GetCardStaticAbilityPlugin : PluginBase
    {
        public override string Name
        {
            get { return "ability command"; }
        }

        public override string Description
        {
            get { return "Get a random card where the rules text starts with the specified static ability.";  }
        }

        public override string ShortDescription
        {
             get { return "Get a random card where the rules text starts with the specified static ability.";  }
        }

        public override string Command
        {
            get { return "ability"; }
        }

        public override string HelpCommand
        {
            get { return "help ability"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'ability flying' or 'ability banding'", this.Command); }
        }

        public GetCardStaticAbilityPlugin(
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

            if (command.Arguments.Any())
            {
                Card card = null;

                if (command.Arguments.Length == 1)
                {
                    string ability = command.Arguments[0];

                    if (string.IsNullOrEmpty(ability))
                        return false;

                    card = await this.Services.Store.GetRandomCardWithStaticAbility(ability);
                }

                if (card != null)
                {
                    messenger.SendMessage(card.Img);

                    return true;
                }
            }

            return false;
        }
    }
}
