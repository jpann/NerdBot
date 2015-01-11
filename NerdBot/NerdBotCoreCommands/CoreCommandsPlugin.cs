using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Plugin;

namespace NerdBotCoreCommands
{
    public class CoreCommandsPlugin : PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "Core Commands"; }
        }

        public override string Description
        {
            get { return "Various 'core' commands (e.g. roll, coinflip)";  }
        }

        public CoreCommandsPlugin()
            : base()
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

            // randomartist
            if (command.Cmd.ToLower() == "randomartist")
            {
                if (command.Arguments.Length == 1)
                {
                    Card card = await base.Store.GetRandomCardByArtist(command.Arguments[0]);

                    if (card != null)
                    {
                        messenger.SendMessage(card.Img);
                    }

                }
            }

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

            // coinflip command
            if (command.Cmd.ToLower() == "coinflip")
            {
                string flip = "Heads";

                if ((this.mRandom.Next(0, 100) % 2) == 0)
                    flip = "Heads";
                else
                    flip = "Tails";

                messenger.SendMessage(string.Format("Coin flip: {0}", flip));
            }

            return false;
        }
    }
}
