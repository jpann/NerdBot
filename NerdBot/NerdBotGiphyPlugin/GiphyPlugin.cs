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

namespace NerdBotGiphyPlugin
{
    public class GiphyPlugin : PluginBase
    {
        private const string cGiphyTestKey = "dc6zaTOxFJmzC";
        public override string Name
        {
            get { return "giphy command"; }
        }

        public override string Description
        {
            get { return "Returns a random gif from Giphy using the keyword.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random gif from Giphy using the keyword."; }
        }

        public override string Command
        {
            get { return "giphy"; }
        }

        public override string HelpCommand
        {
            get { return "help giphy"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'giphy awesome'", this.Command); }
        }

        public GiphyPlugin(
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

            string url = "http://api.giphy.com/v1/gifs/random?tag={0}&api_key=dc6zaTOxFJmzC";

            var giphyFetcher = new GiphyFetcher(url, base.HttpClient);

            if (command.Arguments.Any())
            {
                string giphyUrl = null;

                if (command.Arguments.Length == 1)
                {
                    string keyword = command.Arguments[0];

                    if (!string.IsNullOrEmpty(keyword))
                        giphyUrl = await giphyFetcher.GetGiphyGif(keyword);
                }

                if (!string.IsNullOrEmpty(giphyUrl))
                {
                    messenger.SendMessage(giphyUrl);
                }
            }

            return false;
        }
    }
}
