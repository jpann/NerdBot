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
using Newtonsoft.Json;

namespace NerdBotCoreCommands
{
    public class TappedOutFormatCheckPlugin : PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "formats Command"; }
        }

        public override string Description
        {
            get { return "Gets a list of formats the card is valid in from http://validator.tappedout.net/validate/";  }
        }

        public TappedOutFormatCheckPlugin(
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

            // Parse command
            if (command.Cmd.ToLower() == "formats")
            {
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
                        string name = command.Arguments[0];
                        string set = command.Arguments[1];

                        if (string.IsNullOrEmpty(name))
                            return false;

                        if (string.IsNullOrEmpty(set))
                            return false;

                        // Get card using only name
                        card = await this.Store.GetCard(name, set);
                    }

                    if (card != null)
                    {
                        // From tappedout.net, get the formats this card is legal in
                        var tappedOutFormat = new TappedOutFormatChecker(this.HttpClient);
                        string[] formats = await tappedOutFormat.GetFormats(card.Name);

                        string msg = string.Format("No formats available for '{0}", card.Name);

                        if (formats != null)
                        {
                            msg = string.Format("{0} is legal in formats: {1}",
                                card.Name,
                                string.Join(", ", formats));
                            ;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class TappedOutFormat
    {
        public string[] Formats { get; set; }
    }

    public class TappedOutFormatChecker
    {
        private readonly IHttpClient mHttpClient;
        private const string cUrl = @"http://validator.tappedout.net/validate/?cards=[""{0}""]";

        public TappedOutFormatChecker(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            this.mHttpClient = httpClient;
        }

        public async Task<string[]> GetFormats(string cardName)
        {
            if (string.IsNullOrEmpty(cardName))
                throw new ArgumentNullException("cardName");

            try
            {
                string formatJson = await this.mHttpClient.GetAsJson(string.Format(cUrl, cardName));

                var format = JsonConvert.DeserializeObject<TappedOutFormat>(formatJson);

                if (format.Formats == null)
                    return null;

                return format.Formats;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
