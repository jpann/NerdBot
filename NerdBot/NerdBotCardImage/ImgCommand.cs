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

namespace NerdBotCardImage
{
    public class ImgCommand : PluginBase
    {
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
            get { return string.Format("{0} example usage: 'img spore clou%' or 'img fem,spore cloud' or 'img fallen empires,spore %loud'", this.Command); }
        }

        public ImgCommand(
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

                    // Get card using name and set name or code
                    card = await this.Store.GetCard(name, set);
                }

                if (card != null)
                {
                    // Default to high resolution image.
                    string imgUrl = card.ImgHires;
                    if (string.IsNullOrEmpty(imgUrl))
                        imgUrl = card.Img;

                    messenger.SendMessage(imgUrl);

                    return true;
                }
            }

            return false;
        }
    }
}
