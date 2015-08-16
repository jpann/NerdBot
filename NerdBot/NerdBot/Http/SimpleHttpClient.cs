using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleLogging.Core;

namespace NerdBot.Http
{
    public class SimpleHttpClient : IHttpClient
    {
        private const int cTimeOut = 10000;

        private readonly ILoggingService mLogger;

        public SimpleHttpClient(ILoggingService logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mLogger = logger;
        }

        public string Post(string url, string json)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("json");

            try
            {
                // For Mono builds that decline HTTPS connections by default
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true; // **** Always accept
                    };

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Timeout = cTimeOut;
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    using (var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            return result;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, string.Format("Error posting data to url '{0}'", url));

                throw;
            }
        }

        public string GetPageSource(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            try
            {
                using (WebClient wc = new WebClient())
                {
                    string html = wc.DownloadString(url);

                    return html;
                }
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, string.Format("Error getting page source from '{0}'", url));

                return null;
            }
        }

        public async Task<string> GetAsJson(string url)
        {
	        try
	        {
		        using (var httpClient = new HttpClient())
		        {
			        var json = await httpClient.GetStringAsync(url);

			        return json;
		        }
	        }
	        catch (Exception er)
	        {
                this.mLogger.Error(er, string.Format("Error getting json result from '{0}'", url));

		        return null;
	        }
        }
    }
}
