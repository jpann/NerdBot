using System;
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
    public class RandomFlavorTextPlugin: PluginBase
    {
        public override string Name
        {
            get { return "flavor command"; }
        }

        public override string Description
        {
            get { return "Returns a random card's flavor text.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card's flavor text."; }
        }

        public override string Command
        {
            get { return "flavor"; }
        }

        public override string HelpCommand
        {
            get { return "help flavor"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'flavor'", this.Command); }
        }

        public RandomFlavorTextPlugin(
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

            string flavor = await this.Services.Store.GetRandomFlavorText();

            if (!string.IsNullOrEmpty(flavor))
            {
                messenger.SendMessage(flavor);

                return true;
            }

            return false;
        }
    }
}
