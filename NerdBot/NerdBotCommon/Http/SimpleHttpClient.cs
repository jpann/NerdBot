using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleLogging.Core;

namespace NerdBotCommon.Http
{
    public class SimpleHttpClient : IHttpClient
    {
        private const int cTimeOut = 10000;

        private readonly ILoggingService mLogger;
        private static HttpClient clientCached = new HttpClient(new WebRequestHandler()
        {
            CachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromDays(-1)),
            ReadWriteTimeout = cTimeOut
        });

        private static HttpClient client = new HttpClient();

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

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    using (var streamWriter = new StreamWriter(requestStream))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();

                        using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                        {
                            using (var responseStream = httpResponse.GetResponseStream())
                            {
                                using (var streamReader = new StreamReader(responseStream))
                                {
                                    var result = streamReader.ReadToEnd();
                                    return result;
                                }
                            }
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
                // Added to prevent 'The request was aborted: Could not create SSL/TLS secure channel.' exceptions on Windows 10
                //TODO: Will need to add OS checking
	            //ServicePointManager.Expect100Continue = true;
	            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

	            //clientCached.Timeout = TimeSpan.FromMilliseconds(cTimeOut);
			    var json = await clientCached.GetStringAsync(url);

			    return json;
	        }
	        catch (Exception er)
	        {
                this.mLogger.Error(er, string.Format("Error getting json result from '{0}'", url));

		        return null;
	        }
        }

        public async Task<string> GetAsJsonNonCached(string url)
        {
            try
            {
                // Added to prevent 'The request was aborted: Could not create SSL/TLS secure channel.' exceptions on Windows 10
                //TODO: Will need to add OS checking
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //client.Timeout = TimeSpan.FromMilliseconds(cTimeOut);
                var json = await client.GetStringAsync(url);

                return json;
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, string.Format("Error getting json result from '{0}'", url));

                return null;
            }
        }

        public string GetResponseAsString(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UserAgent = "NerdBot/1.0";
                //request.Timeout = 20;

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var data = new StreamReader(stream).ReadToEnd();
                        stream.Close();


                        return data;
                    }
                }
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, string.Format("Error getting response from '{0}'", url));

                return null;
            }
        }
    }
}
