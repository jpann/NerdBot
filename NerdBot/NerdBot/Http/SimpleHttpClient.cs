using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ninject.Extensions.Logging;

namespace NerdBot.Http
{
    public class SimpleHttpClient : IHttpClient
    {
        private readonly ILogger mLogger;

        public SimpleHttpClient(ILogger logger)
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
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Error posting data to url '{0}'", url);

                throw;
            }
        }
    }
}
