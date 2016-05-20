using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.UrlShortners;

namespace NerdBotCoreCommands
{
    public class GetSetPlugin : PluginBase
    {
        public override string Name
        {
            get { return "set command"; }
        }

        public override string Description
        {
            get { return "Gets set information.";  }
        }

        public override string ShortDescription
        {
            get { return "Gets set information."; }
        }

        public override string Command
        {
            get { return "set"; }
        }

        public override string HelpCommand
        {
            get { return "help set"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'set command% 2013' or 'set c13'", this.Command); }
        }

        public GetSetPlugin(
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
                Set set = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    // Get card using only name
                    if (name.Length == 3)
                    {
                        set = await this.Store.GetSetByCode(name);
                    }
                    else
                    {
                        set = await this.Store.GetSet(name);
                    }
                }

                if (set != null)
                {
                    string msg =
                        string.Format("Set '{0}' [{1}] {2}was released on {3} and contains {4} cards.", 
                            set.Name,
                            set.Code,
                            !string.IsNullOrEmpty(set.Block) ? string.Format(" in block '{0}' ", set.Block) : "",
                            set.ReleasedOn.ToString("MM-dd-yyyy"),
                            set.TotalQty);

                    messenger.SendMessage(msg);
                    return true;
                }
            }

            return false;
        }
    }
}
