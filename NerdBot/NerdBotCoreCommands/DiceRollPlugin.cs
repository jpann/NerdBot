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

namespace NerdBotCoreCommands
{
    public class DiceRollPlugin: PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "roll command"; }
        }

        public override string Description
        {
            get { return "Rolls a die and returns the result.";  }
        }

        public override string ShortDescription
        {
            get { return "Rolls a die and returns the result."; }
        }

        public override string Command
        {
            get { return "roll"; }
        }

        public override string HelpCommand
        {
            get { return "help roll"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'roll' or 'roll 100'", this.Command); }
        }

        public DiceRollPlugin(
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

            int max = 6;

            // Roll 1 through argument value, if argument is an integer
            if (command.Arguments.Length == 1)
            {
                int n;
                bool isNumeric = int.TryParse(command.Arguments[0], out n);

                if (isNumeric)
                    max = n;
            }

            int roll = this.mRandom.Next(1, max);

            messenger.SendMessage(string.Format("Roll 1-{0}: {1}", max, roll));

            return true;
        }
    }
}
