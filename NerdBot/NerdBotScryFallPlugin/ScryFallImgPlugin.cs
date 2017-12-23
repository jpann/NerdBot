using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Plugin;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;

namespace NerdBotScryFallPlugin
{
    public class ScryFallImgPlugin : PluginBase
    {
        private const string cSearchUrl = "https://scryfall.com/search?q=name:/{0}/";

        private ScryFallFetcher fetcher;

        public override string Name
        {
            get { return "simg command"; }
        }

        public override string Description
        {
            get { return "Returns a link to the card's image from Scryfall.com"; }
        }

        public override string ShortDescription
        {
            get { return "Returns a link to the card's image from Scryfall.com"; }
        }

        public override string Command
        {
            get { return "simg"; }
        }

        public override string HelpCommand
        {
            get { return "help simg"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'simg spore clou%' or 'simg fem;spore cloud' or 'simg fallen empires;spore %loud'", this.Command); }
        }

        public ScryFallImgPlugin(
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

                    // Get card from scryfall
                    var scryCard = await fetcher.GetCard(card.MultiverseId);

                    if (scryCard != null)
                    {
                        // Get image, starting with large
                        string img_url;
                        if (!string.IsNullOrEmpty(scryCard.Image_Uris.Png))
                            img_url = scryCard.Image_Uris.Png;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Large))
                            img_url = scryCard.Image_Uris.Large;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Normal))
                            img_url = scryCard.Image_Uris.Normal;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Small))
                            img_url = scryCard.Image_Uris.Small;
                        else
                        {
                            img_url = card.Img;
                        }

                        messenger.SendMessage(img_url);

                        // Get other sets card is in
                        List<Set> otherSets = await this.Services.Store.GetCardOtherSets(card.MultiverseId);
                        if (otherSets.Any())
                        {
                            string msg = string.Format("Also in sets: {0}",
                                string.Join(", ", otherSets.Select(s => s.Code).Take(10).ToArray()));

                            messenger.SendMessage(msg);
                        }
                    }

                    return true;
                }
                else
                {
                    this.mLoggingService.Warning("Couldn't find card using arguments.");

                    string name = "";
                    if (command.Arguments.Length == 1)
                        name = command.Arguments[0];
                    else
                        name = command.Arguments[1];

                    name = Uri.EscapeDataString(name);

                    string url = string.Format(cSearchUrl, name);

                    messenger.SendMessage(string.Format("Try seeing if your card is here: {0}", url));
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