using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Json;
using NerdBot.Http;

namespace NerdBot.Messengers.GroupMe
{
    public class GroupMeMessenger : IMessenger
    {
        private readonly IHttpClient mHttpClient;
        private readonly string mBotId;
        private readonly string mBotName;
        private readonly string mEndpointUrl;
        private readonly string[] mIgnoreNames;

        public string BotId
        {
            get { return this.mBotId; }
        }

        public string BotName
        {
            get { return this.mBotName; }
        }

        public string[] IgnoreNames
        {
            get { return this.mIgnoreNames; }
        }

        public GroupMeMessenger(
            string botId,
            string botName,
            string[] ignoreNames,
            string endPointUrl,
            IHttpClient httpClient)
        {
            if (string.IsNullOrEmpty(botId))
                throw new ArgumentException("botId");

            if (string.IsNullOrEmpty(botName))
                throw new ArgumentException("botName");

            if (string.IsNullOrEmpty(endPointUrl))
                throw new ArgumentException("endPointUrl");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            this.mBotId = botId;
            this.mBotName = botName;
            this.mIgnoreNames = ignoreNames;
            this.mEndpointUrl = endPointUrl;
            this.mHttpClient = httpClient;
        }

        public bool SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            string json = new JavaScriptSerializer().Serialize(new
            {
                text = message,
                bot_id = this.mBotId
            });

            try
            {
                string result = this.mHttpClient.Post(this.mEndpointUrl, json);

                return true;
            }
            catch (Exception er)
            {
                //TODO Logging

                return false;
            }
        }
    }
}
