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
    public class RandomArtistPlugin : PluginBase
    {
        public override string Name
        {
            get { return "randomartist command"; }
        }

        public override string Description
        {
            get { return "Returns a random card where the specific name is the card's artist.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card where the specific name is the card's artist."; }
        }

        public override string Command
        {
            get { return "randomartist"; }
        }

        public override string HelpCommand
        {
            get { return "help randomartist"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: randomartist amy w%ber", this.Command); }
        }

        public RandomArtistPlugin(
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

            if (command.Arguments.Length == 1)
            {
                Card card = await this.Services.Store.GetRandomCardByArtist(command.Arguments[0]);

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
