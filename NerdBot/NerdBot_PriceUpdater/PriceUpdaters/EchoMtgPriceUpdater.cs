using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NerdBot.Http;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using SimpleLogging.Core;

namespace NerdBot_PriceUpdater.PriceUpdaters
{
    public class EchoMtgPriceUpdater : IPriceUpdater
    {
        private const string cUrl = "https://www.echomtg.com/set/{0}/";

        private readonly ICardPriceStore mPriceStore;
        private readonly ILoggingService mLoggingService;
        private readonly IHttpClient mHttpClient;

        public EchoMtgPriceUpdater(
            ICardPriceStore priceStore, 
            IHttpClient httpClient,
            ILoggingService loggingService)
        {
            if (priceStore == null)
                throw new ArgumentNullException("priceStore");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mPriceStore = priceStore;
            this.mHttpClient = httpClient;
            this.mLoggingService = loggingService;
        }

        public void UpdatePrices(Set set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            string url = string.Format(cUrl, set.Code);

            this.mLoggingService.Info("Getting page for set '{0}' from '{1}'...",
                set.Code,
                url);

            string pageSoruce = this.mHttpClient.GetPageSource(url);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSoruce);

            HtmlNode cardsNode = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div[2]/div[2]/div[1]/div[2]/div/table/tbody");

            if (cardsNode == null)
                return;

            if (cardsNode.SelectSingleNode("tr") == null)
            {
                string msg = string.Format("No cards for set '{0}'; skipping...", set.Code);

                Console.WriteLine(msg);
                this.mLoggingService.Warning(msg);

                return;
            }

            foreach (HtmlNode row in cardsNode.SelectNodes("tr"))
            {
                string href = row.Attributes["href"].Value.Trim();

                HtmlNode nameNode = row.SelectSingleNode("td[2]");
                HtmlNode diffNode = row.SelectSingleNode("td[3]");
                HtmlNode midNode = row.SelectSingleNode("td[4]");
                HtmlNode lowNode = row.SelectSingleNode("td[5]");
                HtmlNode foilNode = row.SelectSingleNode("td[6]");

                CardPrice price = new CardPrice();
                price.SetCode = set.Code;
                price.Name = nameNode.InnerText;
                price.SearchName = GetSearchValue(price.Name, false);
                price.PriceDiff = diffNode.InnerText;
                price.PriceFoil = foilNode.InnerText;
                price.PriceLow = lowNode.InnerText;
                price.PriceMid = midNode.InnerText;
                price.Url = "https://www.echomtg.com" + href;
                price.LastUpdated = DateTime.Now;

                string msg = string.Format("Inserting '{0}' from '{1}'... ",
                    price.Name,
                    price.SetCode);

                Console.WriteLine(msg);
                this.mLoggingService.Debug(msg);

                CardPrice card = this.mPriceStore.FindAndModifyCardPrice(price, true);
            }
        }

        public string GetSearchValue(string text, bool forRegex = false)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^]");

            string searchValue = text.ToLower();

            if (forRegex)
            {
                // Replace * and % with a regex '.' char
                searchValue = searchValue.Replace("*", ".");
                searchValue = searchValue.Replace("%", ".");

                // If the first character of the searchValue is not '.', 
                // meaning the user does not want to do a contains search,
                // explicitly use a starts with regex
                if (!searchValue.StartsWith("."))
                {
                    searchValue = "^" + searchValue;
                }
            }

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }
    }
}
