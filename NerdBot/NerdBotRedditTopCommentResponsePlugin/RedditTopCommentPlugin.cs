using NerdBot.Parsers;
using NerdBot.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.UrlShortners;

namespace NerdBotRedditTopCommentResponsePlugin
{
    public class RedditTopCommentPlugin : MessagePluginBase
    {
        private const string cSubReddit = "r/RoastMe";
        private const int cReplyChance = 5;

        private RedditTopFetcher mFetcher;
        private Random mRandom;

        public override string Name
        {
            get { return "Reddit Top Reply"; }
        }

        public override string Description
        {
            get { return "Randomly reply to message with top comment in a given subreddit."; }
        }

        public override string ShortDescription
        {
            get { return "Randomly reply to message with top comment in a given subreddit."; }
        }

        public RedditTopCommentPlugin(
                IBotServices services)
            : base(
                services)
        {
        }

        public override void OnLoad()
        {
            this.mFetcher = new RedditTopFetcher(this.Services.HttpClient);
            this.mRandom = new Random();
        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnMessage(IMessage message, IMessenger messenger)
        {
            // Exit if message was sent by the bot
            if (message.name.ToLower() == this.BotName.ToLower())
                return false;

            // If a message contains 'roast me', get a random r/roastme top comment
            if (message.text.ToLower().Contains("roast me"))
            {
                string reply = await this.mFetcher.GetTopCommentFromSubreddit(cSubReddit);

                // This is lame, but check if the reply conatins url syntax. 
                // If it does, get a new one.
                if (reply.Contains("["))
                {
                    reply = await this.mFetcher.GetTopCommentFromSubreddit(cSubReddit);
                }

                if (!string.IsNullOrEmpty(reply))
                {
                    string name = "@" + message.name;

                    string msg = string.Format("{0} {1}", name, reply);

                    int start = 0;
                    int end = msg.IndexOf(name) + name.Length;

                    messenger.SendMessageWithMention(msg, (string)message.user_id, start, end);
                }
            }
            else
            {
                if (this.mRandom.Next(1, 101) <= cReplyChance)
                {
                    string reply = await this.mFetcher.GetTopCommentFromSubreddit(cSubReddit);

                    // This is lame, but check if the reply conatins url syntax. 
                    // If it does, get a new one.
                    if (reply.Contains("["))
                    {
                        reply = await this.mFetcher.GetTopCommentFromSubreddit(cSubReddit);
                    }

                    if (!string.IsNullOrEmpty(reply))
                    {
                        string name = "@" + message.name;

                        string msg = string.Format("{0} {1}", name, reply);

                        int start = 0;
                        int end = msg.IndexOf(name) + name.Length;

                        messenger.SendMessageWithMention(msg, (string)message.user_id, start, end);
                    }
                }
            }

            return false;
        }
    }
}
