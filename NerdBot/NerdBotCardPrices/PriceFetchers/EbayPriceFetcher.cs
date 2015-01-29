using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace NerdBotCardPrices.PriceFetchers
{
    //TODO This implementation is horrible
    public class EbayPriceFetcher
    {
        public EbayPriceFetcher()
        {
        }

        public string GetPageSource(string url)
        {
            using (WebClient wc = new WebClient())
            {
                string html = wc.DownloadString(url);

                return html;
            }
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

            string pageSource = this.GetPageSource(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            HtmlNode priceNode = htmlDoc.DocumentNode.SelectSingleNode(@"//li[@class=""lvprice prc""]/span");

            if (priceNode != null)
            {
                return new string[]
                {
                    priceNode.InnerText.Trim(),
                    url
                };
            }
            else
                return null;
        }
    }
}
