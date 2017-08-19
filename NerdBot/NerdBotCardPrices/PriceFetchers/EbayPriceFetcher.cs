using System;
using HtmlAgilityPack;
using NerdBotCommon.Http;
using SimpleLogging.Core;

namespace NerdBotCardPrices.PriceFetchers
{
    public class EbayPriceFetcher
    {
        private IHttpClient mHttpClient;
        private ILoggingService mLoggingService;

        public EbayPriceFetcher(IHttpClient httpClient, ILoggingService loggingService)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mHttpClient = httpClient;
            this.mLoggingService = loggingService;
        }


        public string[] GetPrice(string name, string setName = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            string url = "http://www.ebay.com/sch/i.html?_sacat=0&_sop=15&LH_BIN=1&_nkw=" + Uri.EscapeDataString(name);

            if (!string.IsNullOrEmpty(setName))
            {
                url += Uri.EscapeDataString(" " + setName);
            }

            url += Uri.EscapeDataString(" mtg nm");

            string pageSource = this.mHttpClient.GetPageSource(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            HtmlNode priceNode = htmlDoc.DocumentNode.SelectSingleNode(@"//li[@class=""lvprice prc""]/span");

            if (priceNode != null)
            {
                string price = priceNode.InnerText.Trim();

                this.mLoggingService.Trace("Price '{0}' found in url '{1}' for name '{2}' and set '{3}'",
                    price, url, name, setName);

                return new string[]
                {
                    price,
                    url
                };
            }
            else
            {
                this.mLoggingService.Warning("No 'price node' in url '{0}'", url);

                return null;
            }
        }
    }
}
