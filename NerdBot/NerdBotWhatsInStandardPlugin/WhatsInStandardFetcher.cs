using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Http;
using Newtonsoft.Json;

namespace NerdBotWhatsInStandardPlugin
{
    public class WhatsInStandardData
    {
        [JsonProperty("deprecated")]
        public bool Deprecated { get; set; }

        [JsonProperty("sets")]
        public List<WhatsInStandardSetData> Sets { get; set; }
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
        private const string cUrl = "http://whatsinstandard.com/api/v5/sets.json";

        private readonly IHttpClient mHttpClient;


        public WhatsInStandardFetcher(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            this.mHttpClient = httpClient;
        }

        public async Task<WhatsInStandardData> GetDataAsync(bool filter = true)
        {
            try
            {
                string latestJson = await this.mHttpClient.GetAsJson(cUrl);

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                var def = JsonConvert.DeserializeObject<WhatsInStandardData>(latestJson);

                if (def == null)
                    return null;

                // Filter out sets that haven't entered standard yet or have already exited standard
                if (filter)
                {
                    var today = DateTime.Now;

                    def.Sets.RemoveAll(s =>
                        ((s.EnterDate ?? DateTime.MaxValue) > today) || ((s.ExitDate ?? DateTime.MaxValue) < today));
                }

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
