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
    public class CardSetsListPlugin : PluginBase
    {
        public override string Name
        {
            get { return "cardsets command"; }
        }

        public override string Description
        {
            get { return "Lists the sets the card appears in.";  }
        }

        public override string ShortDescription
        {
            get { return "Lists the sets the card appears in."; }
        }

        public override string Command
        {
            get { return "cardsets"; }
        }

        public override string HelpCommand
        {
            get { return "help cardsets"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0}: HELP TEXT HERE", this.Command); }
        }

        public CardSetsListPlugin(
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
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    // Get card using only name
                    card = await this.Store.GetCard(name);
                }
                else if (command.Arguments.Length == 2)
                {
                    string name = command.Arguments[1];
                    string set = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    // Get card using only name
                    card = await this.Store.GetCard(name, set);
                }

                if (card != null)
                {
                    List<Set> sets = await this.Store.GetCardSets(card.MultiverseId);

                    if (sets.Any())
                    {
                        string[] setNames = sets.Select(s => string.Format("{0} [{1}]", s.Name, s.Code)).ToArray();

                        string msg = string.Format("{0} appears in sets: {1}",
                           card.Name,
                           string.Join(", ", setNames));

                        messenger.SendMessage(msg);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
