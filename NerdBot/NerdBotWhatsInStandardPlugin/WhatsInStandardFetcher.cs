using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Http;
using Newtonsoft.Json;

namespace NerdBotWhatsInStandardPlugin
{
    public class WhatsInStandardData
    {
        
    }

    public class WhatsInStandardSetData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("block")]
        public string Block { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("enter_date")]
        public DateTime? EnterDate { get; set; }

        [JsonProperty("exit_date")]
        public DateTime? ExitDate { get; set;  }

        [JsonProperty("rough_exit_date")]
        public string RoughExitDate { get; set; }
    }

    public class WhatsInStandardFetcher
    {
        private const string cUrl = "http://whatsinstandard.com/api/4/sets.json";

        private readonly IHttpClient mHttpClient;


        public WhatsInStandardFetcher(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mHttpClient = httpClient;
        }

        public async Task<List<WhatsInStandardSetData>> GetData()
        {
            try
            {
                string latestJson = await this.mHttpClient.GetAsJson(cUrl);

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                var def = JsonConvert.DeserializeObject<List<WhatsInStandardSetData>>(latestJson);

                if (def == null)
                    return null;

                return def;
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
