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

			string url = "http://api.giphy.com/v1/gifs/translate?s={0}&api_key=dc6zaTOxFJmzC";

			try
			{
	            var giphyFetcher = new GiphyFetcher(url, this.Services.HttpClient);

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
			catch (Exception er) 
			{
				return false;
			}
        }
    }
}
