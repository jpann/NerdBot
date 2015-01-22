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
        public override string Name
        {
            get { return "formats Command"; }
        }

        public override string Description
        {
            get { return "Gets a list of formats the card is valid in from http://validator.tappedout.net/validate/";  }
        }

        public override string ShortDescription
        {
            get { return "Gets a list of formats the card is valid in from http://validator.tappedout.net/validate/"; }
        }

        public override string Command
        {
            get { return "formats"; }
        }

        public override string HelpCommand
        {
            get { return "help formats"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'formats spore clou%' or 'formats fem,spore clou%' or 'formats fallen empires,spore clou%'", this.Command); }
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
                    // From tappedout.net, get the formats this card is legal in
                    var tappedOutFormat = new TappedOutFormatChecker(this.HttpClient);
                    string[] formats = await tappedOutFormat.GetFormats(card.Name);
                    
                    if (formats != null)
                    {
                        string msg = string.Format("{0} is legal in formats: {1}",
                            card.Name,
                            string.Join(", ", formats));

                        messenger.SendMessage(msg);

                        return true;
                    }
                    else
                    {
                        string msg = string.Format("No formats available for '{0}' or http://validator.tappedout.net/validate/ is offline.", card.Name);

                        messenger.SendMessage(msg);

                        return true;
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

                if (string.IsNullOrEmpty(formatJson))
                    return null;

                var format = JsonConvert.DeserializeObject<TappedOutFormat>(formatJson);

                if (format.Formats == null)
                    return null;

                return format.Formats;
            }
            catch (Exception er)
            {
                Console.WriteLine("ERROR getting format: {0}", er.Message);

                throw;
            }
        }

    }
}
