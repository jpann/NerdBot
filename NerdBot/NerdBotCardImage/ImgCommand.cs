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
using NerdBotCommon.Extensions;

namespace NerdBotCardImage
{
    public class ImgCommand : PluginBase
    {
		private const string cSearchUrl = "{0}/search/{1}";

        public override string Name
        {
            get { return "img command"; }
        }

        public override string Description
        {
            get { return "Returns a link to the card's image.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a link to the card's image."; }
        }

        public override string Command
        {
            get { return "img"; }
        }

        public override string HelpCommand
        {
            get { return "help img"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'img spore clou%' or 'img fem;spore cloud' or 'img fallen empires;spore %loud'", this.Command); }
        }

        public ImgCommand(
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

            if (command.Arguments.Any())
            {
                Card card = null;

                string searchTerm = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    this.mLoggingService.Trace("Using Name: {0}", name);

                    searchTerm = name;

                    // Get card using only name
                    card = await this.Services.Store.GetCard(name);
                }
                else if (command.Arguments.Length == 2)
                {
                    string name = command.Arguments[1];
                    string set = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    this.mLoggingService.Trace("Using Name: {0}; Set: {1}", name, set);

                    searchTerm = string.Join(" ", command.Arguments);

                    // Get card using name and set name or code
                    card = await this.Services.Store.GetCard(name, set);

                }

                if (card != null)
                {
                    this.mLoggingService.Trace("Found card '{0}' in set '{1}'.",
                        card.Name,
                        card.SetName);

                    this.Services.QueryStatisticsStore.InsertCardQueryStat(message.name, message.user_id,
                        card.MultiverseId, searchTerm);

                    // Default to high resolution image.
                    string imgUrl = card.ImgHires;
                    if (string.IsNullOrEmpty(imgUrl))
                        imgUrl = card.Img;

                    messenger.SendMessage(imgUrl);

                    // Get other sets card is in
                    List<Set> otherSets = await this.Services.Store.GetCardOtherSets(card.MultiverseId);
                    if (otherSets.Any())
                    {
                        string msg = string.Format("Also in sets: {0}",
                            string.Join(", ", otherSets.Select(s => s.Code).Take(10).ToArray()));
                        
                        messenger.SendMessage(msg);
                    }

                    return true;
                }
                else
                {
                    this.mLoggingService.Warning("Couldn't find card using arguments.");

                    // Use autocomplete to try returning a list of suggested names
                    string name = "";
                    if (command.Arguments.Length == 1)
                        name = command.Arguments[0];
                    else
                        name = command.Arguments[1];

                    // Get first 5 characters of name to use with autocomplete
                    string autocompleteName = new string(name.Take(5).ToArray());

                    var autocompleteResults = await this.Services.Autocompleter.GetAutocompleteAsync(autocompleteName);
                    if (autocompleteResults != null && autocompleteResults.Any())
                    {
                        LoggingService.Trace("Autocomplete returned '{0}' results for '{1}'...", autocompleteResults.Count(), name);

                        string suggestions = autocompleteResults.Take(5).OxbridgeOr();

                        string msg = string.Format("Did you mean {0}?", suggestions);

                        messenger.SendMessage(msg);
                    }
                    else
                    {
                        name = Uri.EscapeDataString(name);

                        string url = string.Format(cSearchUrl, this.mBotConfig.HostUrl, name);

                        messenger.SendMessage(string.Format("Try seeing if your card is here: {0}", url));
                    }
                }
            }
            else
            {
                this.mLoggingService.Warning("No arguments provided.");
            }

            return false;
        }
    }
}
