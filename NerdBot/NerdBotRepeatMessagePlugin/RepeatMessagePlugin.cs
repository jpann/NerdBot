using System;
using System.Linq;
using System.Threading.Tasks;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.UrlShortners;

namespace NerdBotRepeatMessagePlugin
{
    public class RepeatMessagePlugin : MessagePluginBase
    {
        private const int cQueueLimit = 5; // The message queue limit
        private const int cMinRepeatMessageCount = 3; // Number of repeated messages in the queue in order to trigger a response

        private LimitedQueue<IMessage> mQueue;

        public override string Name
        {
            get { return "Repeat Message"; }
        }

        public override string Description
        {
            get { return "Detects repeated messages and has fun with it."; }
        }

        public override string ShortDescription
        {
            get { return "Detects repeated messages and has fun with it."; }
        }

        public RepeatMessagePlugin(
                IMtgStore store,
                ICardPriceStore priceStore,
                ICommandParser commandParser,
                IHttpClient httpClient,
                IUrlShortener urlShortener)
            : base(
                store,
                priceStore,
                commandParser,
                httpClient,
                urlShortener)
        {
        }

        public override void OnLoad()
        {
            this.mQueue = new LimitedQueue<IMessage>(cQueueLimit);
        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnMessage(IMessage message, IMessenger messenger)
        {
            // Exit if message was sent by the bot
            if (message.name.ToLower() == this.BotName.ToLower())
                return false;

            this.mQueue.Enqueue(message);

            // Get last X queue items
            var items = this.mQueue.Reverse().Take(cMinRepeatMessageCount).Reverse().ToList();

            int count = items.Where(i => i.text.ToLower() == message.text.ToLower()).Count();

            if (count >= cMinRepeatMessageCount)
            {
                messenger.SendMessage(message.text);
            }

            return false;
        }
    }
}
