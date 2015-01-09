using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.Plugin;

namespace NerdBotCardImage
{
    public class ImgHiresCommand : IPlugin
    {
        private IMtgStore mStore;
        private ICommandParser mCommandParser;

        public string Name
        {
            get { return "imghires Command"; }
        }

        public string Description
        {
            get { return "Returns a link to the card's image."; }
        }

        public void Load(IMtgStore store, ICommandParser commandParser)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentException("commandParser");

            this.mStore = store;
            this.mCommandParser = commandParser;
        }

        public void Unload()
        {
        }

        public async Task<bool> OnMessage(IMessage message, IMessenger messenger)
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
            if (command.Cmd.ToLower() == "imghires")
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
                       card = await  this.mStore.GetCard(name);
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
                        card = await this.mStore.GetCard(name, set);
                    }

                    if (card != null)
                    {
                        string imgUrl = card.Img;

                        messenger.SendMessage(imgUrl);
                    }
                }
            }

            return false;
        }
    }
}
