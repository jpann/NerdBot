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
using NerdBot.UrlShortners;

namespace NerdBotCoreCommands
{
    public class DiceRollPlugin: PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "roll Commands"; }
        }

        public override string Description
        {
            get { return "Rolls a die and returns the result.";  }
        }

        public DiceRollPlugin(
                IMtgStore store,
                ICommandParser commandParser,
                IHttpClient httpClient,
                IUrlShortener urlShortener)
            : base(
                store,
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
            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            var command = this.mCommandParser.Parse(message.text);

            // If there was no command, return
            if (command == null)
                return false;

            // roll command
            if (command.Cmd.ToLower() == "roll")
            {
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
            }

            return false;
        }
    }
}
