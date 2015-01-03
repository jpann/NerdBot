using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Plugin;

namespace NerdBotCardImage
{
    public class ImgCommand : IPlugin
    {
        private IMtgStore mStore;

        public string Name
        {
            get { return "img Command"; }
        }

        public string Description
        {
            get { return "Returns a link to the card's image.";  }
        }

        public void Load(IMtgStore store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            this.mStore = store;
        }

        public void Unload()
        {
        }

        public bool OnMessage(IMessage message, IMessenger messenger)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            // Parse command
            if (message.text.StartsWith("img "))
            {
                string name = message.text.Substring(message.text.IndexOf(" "));

                if (string.IsNullOrEmpty(name))
                    return false;

                // Get card
                Card card = this.mStore.GetCard(name);

                if (card != null)
                {
                    string imgUrl = card.Img;

                    messenger.SendMessage(imgUrl);
                }
            }

            return false;
        }
    }
}
