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
    public class CoinFlipPlugin : PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "coinflip command"; }
        }

        public override string Description
        {
            get { return "Clips a coin and returns the result.";  }
        }

        public override string ShortDescription
        {
            get { return "Clips a coin and returns the result."; }
        }

        public override string Command
        {
            get { return "coinflip"; }
        }

        public override string HelpCommand
        {
            get { return "help coinflip"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'coinflip'", this.Command); }
        }

        public CoinFlipPlugin(
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
            this.mRandom = new Random();
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

            string flip = "Heads";

            if ((this.mRandom.Next(0, 100) % 2) == 0)
                flip = "Heads";
            else
                flip = "Tails";

            messenger.SendMessage(string.Format("Coin flip: {0}", flip));

            return true;
        }
    }
}
