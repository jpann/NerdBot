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

namespace NerdBotCoreCommands
{
    public class RandomArtistPlugin : PluginBase
    {
        public override string Name
        {
            get { return "randomartist command."; }
        }

        public override string Description
        {
            get { return "Returns a random card where the specific name is the card's artist.";  }
        }

        public RandomArtistPlugin(
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
            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            var command = this.mCommandParser.Parse(message.text);

            // If there was no command, return
            if (command == null)
                return false;

            // randomartist
            if (command.Cmd.ToLower() == "randomartist")
            {
                if (command.Arguments.Length == 1)
                {
                    Card card = await base.Store.GetRandomCardByArtist(command.Arguments[0]);

                    if (card != null)
                    {
                        messenger.SendMessage(card.Img);
                    }

                }
            }


            return false;
        }
    }
}
