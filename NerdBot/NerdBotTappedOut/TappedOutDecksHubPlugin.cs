using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using NerdBotTappedOut.Fetchers;

namespace NerdBotTappedOut
{
    public class TappedOutDecksHubPlugin : PluginBase
    {
        private const int cLimit = 8;

        public override string Name
        {
            get { return "decks command"; }
        }

        public override string Description
        {
            get { return string.Format("Returns the first {0} decks from the specified deck hub on tappedout.net", cLimit); }
        }

        public override string ShortDescription
        {
            get { return string.Format("Returns the first {0} decks from the specified deck hub on tappedout.net", cLimit); }
        }

        public override string Command
        {
            get { return "decks"; }
        }

        public override string HelpCommand
        {
            get { return "help decks"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0}: 'decks aggro' or 'decks tokens'", this.Command); }
        }

        public TappedOutDecksHubPlugin(
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

            if (command.Arguments.Length >= 1)
            {
                string listSlug = command.Arguments[0];

                Regex rgx = new Regex("[^a-zA-Z0-9- ]");
                listSlug = rgx.Replace(listSlug, "");
                listSlug = listSlug.ToLower().Replace(" ", "-");

                string url = string.Format("http://tappedout.net/api/deck/latest/{0}/", listSlug);

                var latestFetcher = new TappedOutLatestFetcher(url, this.Services.HttpClient);
                List<TappedOutLatestDeckData> latestData = await latestFetcher.GetLatest();

                if (latestData != null)
                {
                    if (latestData.Any())
                    {
                        int total = latestData.Count;

                        if (command.Arguments.Length == 2)
                        {
                            string filter = command.Arguments[1].ToLower();

                            latestData = latestData.Where(l => l.Name.ToLower().Contains(filter)).ToList();
                        }

                        string[] latestDecks = latestData.Select(s =>
                                string.Format("{0} [{1}]", s.Name, this.Services.UrlShortener.ShortenUrl(s.Url)))
                            .Take(cLimit)
                            .ToArray();

                        string msg = string.Format("{0} decks: {1} [{2}/{3}]",
                            listSlug,
                            string.Join(", ", latestDecks),
                            latestDecks.Count(),
                            total);

                        messenger.SendMessage(msg);

                        return true;
                    }
                    else
                    {
                        messenger.SendMessage(string.Format("No deck hub found for '{0}'... Check http://tappedout.net/mtg-deck-builder/ for the hub name.", listSlug));
                    }
                }
                else
                {
                    messenger.SendMessage(string.Format("No deck hub found for '{0}'... Check http://tappedout.net/mtg-deck-builder/ for the hub name.", listSlug));
                }
            }

            return false;
        }
    }
}
