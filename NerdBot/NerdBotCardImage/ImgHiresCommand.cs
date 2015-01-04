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
    public class ImgHiresCommand : IPlugin
    {
        private IMtgStore mStore;

        public string Name
        {
            get { return "imghires Command"; }
        }

        public string Description
        {
            get { return "Returns a link to the card's image."; }
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
            if (message.text.StartsWith("imghires "))
            {
                string name = message.text.Substring(message.text.IndexOf(" ") + 1);

                if (string.IsNullOrEmpty(name))
                    return false;

                // Get card
                Card card = this.mStore.GetCard(name);

                if (card != null)
                {
                    string imgUrl = card.Img_Hires;

                    messenger.SendMessage(imgUrl);
                }
            }

            return false;
        }
    }
}
