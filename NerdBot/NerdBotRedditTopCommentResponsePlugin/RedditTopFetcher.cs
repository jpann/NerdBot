using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Http;

namespace NerdBotRedditTopCommentResponsePlugin
{
    public class RedditTopFetcher
    {
        private const string cUrl = "https://www.reddit.com/{0}/random/.json?sort=%27top%27&limit=1";

        private readonly IHttpClient mHttpClient;

        public RedditTopFetcher(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mHttpClient = httpClient;
        }

        public async Task<string> GetTopCommentFromSubreddit(string subreddit)
        {
            string text = null;

            try
            {
                string data = this.mHttpClient.GetResponseAsString(string.Format(cUrl, subreddit));

                if (data != null)
                {

                    var json = JArray.Parse(data);

                    if (json != null)
                    {
                        text = json.Last()["data"]["children"][0]["data"]["body"].ToString();
                    }
                }                

                return text;
            }
            catch (Exception er)
            {
                string msg = string.Format("ERROR getting data",
                    er.Message);

                Console.WriteLine(msg);

                throw;
            }
        }
    }
}
