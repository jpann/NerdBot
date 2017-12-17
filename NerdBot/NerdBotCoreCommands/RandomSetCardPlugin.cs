using System;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Plugin;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;

namespace NerdBotCoreCommands
{
    public class RandomSetCardPlugin : PluginBase
    {
        public override string Name
        {
            get { return "setrandom command"; }
        }

        public override string Description
        {
            get { return "Returns a random card from the specified set.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card from the specified set."; }
        }

        public override string Command
        {
            get { return "setrandom"; }
        }

        public override string HelpCommand
        {
            get { return "help setrandom"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'setrandom fem' or 'setrandom fifth edition'", this.Command); }
        }

        public RandomSetCardPlugin(
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
                Card card = await this.Services.Store.GetRandomCardInSet(command.Arguments[0]);

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
