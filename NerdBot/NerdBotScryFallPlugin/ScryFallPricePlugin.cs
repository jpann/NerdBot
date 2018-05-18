using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Plugin;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotCommon.Extensions;
using NerdBotScryFallPlugin.POCO;

namespace NerdBotScryFallPlugin
{
    public class ScryFallPricePlugin : PluginBase
    {
        private const string cSearchUrl = "https://scryfall.com/search?q=name:/{0}/";

        private ScryFallFetcher fetcher;

        public override string Name
        {
            get { return "scry command"; }
        }

        public override string Description
        {
            get { return "Returns the card's USD price from scryfall.com."; }
        }

        public override string ShortDescription
        {
            get { return "Returns the card's USD price from scryfall.com."; }
        }

        public override string Command
        {
            get { return "scry"; }
        }

        public override string HelpCommand
        {
            get { return "help scry"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'scry spore clou%' or 'scry fem;spore cloud' or 'scry fallen empires;spore %loud'", this.Command); }
        }

        public ScryFallPricePlugin(
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
            fetcher = new ScryFallFetcher(this.Services.HttpClient);
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
                ScryFallCard scryCard = null;

                string searchTerm = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    this.mLoggingService.Trace("Using Name: {0}", name);

                    searchTerm = name;

                    // Get card using only name
                    scryCard = await fetcher.GetCard(name, true);
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
                    scryCard = await fetcher.GetCard(name, set, true);
                }

                if (scryCard != null)
                {
                    this.mLoggingService.Trace("Found card '{0}' in set '{1}'.",
                        scryCard.Name,
                        scryCard.SetName);

                    await this.Services.QueryStatisticsStore.InsertCardQueryStat(message.name, message.user_id,
                        scryCard.MultiverseIds.FirstOrDefault(), searchTerm);

                    string price = scryCard.PriceUsd;
                    string url = scryCard.ScryFallUri;

                    url = this.Services.UrlShortener.ShortenUrl(url);

                    if (!string.IsNullOrEmpty(price))
                    {
                        string msg = string.Format("{0} [{1}] - ${2}. {3}", scryCard.Name, scryCard.SetCode.ToUpper(),
                            price, url);

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

                        string url = string.Format(cSearchUrl, name);

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