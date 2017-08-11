using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Utilities;
using SimpleLogging.Core;

namespace NerdBot_PriceUpdater.PriceUpdaters
{
    public class EchoMtgPriceUpdater : IPriceUpdater
    {
        private const string cUrl = "https://www.echomtg.com/set/{0}/";
        private const string cCardUrl = "https://www.echomtg.com{0}";

        private readonly ICardPriceStore mPriceStore;
        private readonly ILoggingService mLoggingService;
        private readonly IHttpClient mHttpClient;
        private readonly SearchUtility mSearchUtility;

        public EchoMtgPriceUpdater(
            ICardPriceStore priceStore, 
            IHttpClient httpClient,
            ILoggingService loggingService,
            SearchUtility searchUtility)
        {
            if (priceStore == null)
                throw new ArgumentNullException("priceStore");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            if (searchUtility == null)
                throw new ArgumentNullException("searchUtility");

            this.mPriceStore = priceStore;
            this.mHttpClient = httpClient;
            this.mLoggingService = loggingService;
            this.mSearchUtility = searchUtility;
        }

        public void UpdatePrices(Set set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            string url = string.Format(cUrl, set.Code);

            this.mLoggingService.Info("Getting page for set '{0}' from '{1}'...",
                set.Code,
                url);

            string pageSource = this.mHttpClient.GetPageSource(url);

			this.mLoggingService.Trace (pageSource);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

			this.mLoggingService.Debug ("Loaded page into memory");

            // Create set price
            HtmlNode setPrices = htmlDoc.DocumentNode.SelectSingleNode(@"//*[contains(@class,'todaysprices')]");

            if (setPrices != null)
            {
                SetPrice setPrice = new SetPrice();

                setPrice.Name = set.Name;
                setPrice.SetCode = set.Code;
                setPrice.SearchName = set.SearchName;
                setPrice.Url = url;
                setPrice.LastUpdated = DateTime.Now;

                if (setPrices.SelectSingleNode("./div[@class=' mid'][1]/span[@class='numbers price_mid']") != null)
                {
                    string totalCards = setPrices.SelectSingleNode("./div[@class=' mid'][1]/span[@class='numbers price_mid']").InnerText;
                    totalCards = totalCards.Trim();

                    setPrice.TotalCards = Convert.ToInt32(totalCards);
                }

                if (setPrices.SelectSingleNode("./div[@class=' mid'][2]/span[@class='numbers price_mid']") != null)
                {
                    string setComplPrice = setPrices.SelectSingleNode("./div[@class=' mid'][2]/span[@class='numbers price_mid']").InnerText;
                    setComplPrice = setComplPrice.Trim();

                    setPrice.SetValue = setComplPrice;
                }

                if (setPrices.SelectSingleNode("./div[@class='foil']/span[@class='numbers price_low']") != null)
                {
                    string setFoilPrice = setPrices.SelectSingleNode("./div[@class='foil']/span[@class='numbers price_low']").InnerText;
                    setFoilPrice = setFoilPrice.Trim();

                    setPrice.FoilSetValue = setFoilPrice;
                }

                string msg = string.Format("Inserting set price for '{0} [{1}]'... ",
                    setPrice.Name,
                    setPrice.SetCode);

                Console.WriteLine(msg);
                this.mLoggingService.Debug(msg);

                SetPrice newSetPrice = this.mPriceStore.FindAndModifySetPrice(setPrice, true);

                this.mLoggingService.Debug("Saved price for set '{0}'.", setPrice.Name);
            }

            HtmlNode cardsNode = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div[4]/div/div[5]/table/tbody");

			this.mLoggingService.Debug ("Parsed cards node");

            if (cardsNode == null)
            {
                this.mLoggingService.Warning("CardsNode is NULL for '{0}'.", url);

                return;
            }

            if (cardsNode.SelectSingleNode("tr") == null)
            {
                string msg = string.Format("No cards for set '{0}'; skipping...", set.Code);

                Console.WriteLine(msg);
                this.mLoggingService.Warning(msg);

                return;
            }

            foreach (HtmlNode row in cardsNode.SelectNodes("tr"))
            {
				if (row == null) 
				{
					this.mLoggingService.Warning ("Row is NULL");
				}

                //string href = row.Attributes["href"].Value.Trim();

                //HtmlNode nameNode = row.SelectSingleNode("td[2]");
                //HtmlNode diffNode = row.SelectSingleNode("td[3]");
                //HtmlNode midNode = row.SelectSingleNode("td[4]");
                //HtmlNode lowNode = row.SelectSingleNode("td[5]");
                //HtmlNode foilNode = row.SelectSingleNode("td[6]");

                HtmlNode nameNode = row.SelectSingleNode("td[3]");
				if (nameNode == null) 
				{
					this.mLoggingService.Warning ("nameNode is NULL");
				}

                HtmlNode diffNode = row.SelectSingleNode("td[4]");
				if (diffNode == null) 
				{
					this.mLoggingService.Warning ("diffNode is NULL");
				}

                HtmlNode lowNode = row.SelectSingleNode("td[5]");
				if (lowNode == null) 
				{
					this.mLoggingService.Warning ("lowNode is NULL");
				}

                HtmlNode foilNode = row.SelectSingleNode("td[6]");
				if (nameNode == null) 
				{
					this.mLoggingService.Warning ("foilNode is NULL");
				}

                CardPrice price = new CardPrice();
                price.SetCode = set.Code;
                price.Name = nameNode.InnerText;
                price.SearchName = this.mSearchUtility.GetSearchValue(price.Name);

                price.PriceDiff = diffNode.InnerText;
                price.PriceDiffValue = 0;

				this.mLoggingService.Debug ("Card={0}; Set={1}; PriceDiff={2}; PriceDiffVal={3}",
					price.Name, price.SetCode, price.PriceDiff, price.PriceDiffValue);

                // Try to parse PriceDiffValue from PriceDiff
                if (!string.IsNullOrEmpty(price.PriceDiff))
                {
                    if (price.PriceDiff.IndexOf("%") > 0)
                    {
                        try
                        {
                            price.PriceDiffValue = Convert.ToInt32(price.PriceDiff.Substring(0, price.PriceDiff.IndexOf("%")));
                        }
                        catch (Exception)
                        {
                            this.mLoggingService.Warning("Price diff for card '{0}' was not NULL (priceDiff: {5})but did not contain a correct integer.", price.PriceDiff);
                        }
                    }
                }

                string tempPrice = lowNode.InnerText;
                price.PriceLow = "$0";
                price.PriceMid = "$0";
                price.PriceFoil = "$0";

				this.mLoggingService.Debug ("TempPrice={0}", tempPrice);

				if (string.IsNullOrEmpty (tempPrice)) 
				{
					this.mLoggingService.Debug ("TempPrice is NULL");
				}

                if (tempPrice.Split('/').Any())
                {
                    string[] lmPrices = tempPrice.Split('/');

                    price.PriceMid = lmPrices[0].Trim();

                    if (lmPrices.Count() == 2)
                    {
                        price.PriceLow = lmPrices[1].Trim();
                    }
                }

                // Get multiverseId
                int multiverseId = Convert.ToInt32(row.Attributes["data-id"].Value);

                HtmlNode nameChildNode = nameNode.SelectSingleNode("./a");
                if (nameChildNode != null)
                {
                    string cardUrl = string.Format(cCardUrl, nameChildNode.Attributes["href"].Value);
                    string cardImageUrl = nameChildNode.Attributes["data-image"].Value;

                    price.Url = cardUrl;
                    price.ImageUrl = cardImageUrl;
                }
                
                price.PriceFoil = foilNode.InnerText;
                price.LastUpdated = DateTime.Now;
                price.MultiverseId = multiverseId;
   
				this.mLoggingService.Debug ("PriceFoil={0}; PriceLow={1}; PriceMid={2}",
					price.PriceFoil, price.PriceLow, price.PriceMid);

                string msg = string.Format("Inserting '{0}' from '{1}'... ",
                    price.Name,
                    price.SetCode);

                Console.WriteLine(msg);
                this.mLoggingService.Debug(msg);

                CardPrice card = this.mPriceStore.FindAndModifyCardPrice(price, true);

				this.mLoggingService.Debug ("Saved price for card '{0}'.", card.Name);
            }
        }
    }
}
