using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.UrlShortners;

namespace NerdBotWhatsInStandardPlugin
{
    public class WhatsInStandardPlugin: PluginBase
    {
        public override string Name
        {
            get { return "whatsinstandard command"; }
        }

        public override string Description
        {
            get { return "Returns the list of sets currently in Standard format.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns the list of sets currently in Standard format."; }
        }

        public override string Command
        {
            get { return "whatsinstandard"; }
        }

        public override string HelpCommand
        {
            get { return "help whatsinstandard"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'whatsinstandard'", this.Command); }
        }

        public WhatsInStandardPlugin(
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

            var whatsInStandard = new WhatsInStandardFetcher(base.HttpClient);

            List<WhatsInStandardSetData> data = null;

            data = await whatsInStandard.GetData();

            if (data != null)
            {
                // Build message
                string msg = "Current sets in Standard: {0}";


                msg = string.Format(msg, string.Join(", ", data.Select(s => s.Code)));

                messenger.SendMessage(msg);
            }

            return false;
        }
    }
}
