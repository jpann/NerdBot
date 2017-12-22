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
        private const string cSetsListUrl = "https://www.echomtg.com/sets/";

        private readonly IMtgStore mMtgStore;
        private readonly ICardPriceStore mPriceStore;
        private readonly ILoggingService mLoggingService;
        private readonly IHttpClient mHttpClient;
        private readonly SearchUtility mSearchUtility;

        public EchoMtgPriceUpdater(
            IMtgStore mtgStore,
            ICardPriceStore priceStore, 
            IHttpClient httpClient,
            ILoggingService loggingService,
            SearchUtility searchUtility)
        {
            if (mtgStore == null)
                throw new ArgumentNullException("mtgStore");

            if (priceStore == null)
                throw new ArgumentNullException("priceStore");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            if (searchUtility == null)
                throw new ArgumentNullException("searchUtility");

            this.mMtgStore = mtgStore;
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

                // Skip cards that have the pattern (0-9) in the name (e.g. Plains (310)) since this are lands
                if (Regex.IsMatch(nameNode.InnerText, @"\([0-9]+\)"))
                {
                    this.mLoggingService.Warning("Skipping card '{0}' due to invalid pattern.", nameNode.InnerText);

                    continue;
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

        public void UpdatePrices()
        {
            string url = cSetsListUrl;

            this.mLoggingService.Info("Getting page for sets list from '{0}'...", url);

            string pageSource = this.mHttpClient.GetPageSource(url);

            this.mLoggingService.Trace(pageSource);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            this.mLoggingService.Debug("Loaded page into memory");

            HtmlNodeCollection setsNodes = htmlDoc.DocumentNode.SelectNodes(@"//*[contains(@class,'c-13')]");

            if (setsNodes != null)
            {
                foreach (HtmlNode setNode in setsNodes)
                {
                    string setUrlPart = setNode.SelectSingleNode("./h4/a").Attributes["href"].Value;
                    string setNameFull = setNode.SelectSingleNode("./h4/a").InnerText;

                    Match match = Regex.Match(setNameFull, @"(?<set>.+) \((?<code>.+)\)");

                    if (match.Success)
                    {
                        string name = match.Groups["set"].Value;
                        string setCode = match.Groups["code"].Value;

                        string setUrl = setUrlPart;
                        if (!setUrlPart.StartsWith("https://www.echomtg.com/set/"))
                        {
                            setUrl = string.Format(cCardUrl, setUrlPart);
                        }

                        // Make sure none of these values are empty or null
                        if (!string.IsNullOrEmpty(name) && 
                            !string.IsNullOrEmpty(setCode) &&
                            !string.IsNullOrEmpty(setUrlPart))
                        {
                            name = name.Trim();
                            this.mLoggingService.Debug("Found Set '{0}' [{1}]; Url = {2}", name, setCode, setUrl);

                            var set = this.mMtgStore.GetSet(name).Result;

                            if (set == null)
                            {
                                this.mLoggingService.Warning("Set named '{0}' [{1}] has no set in the database with that name. Trying code instead...", name, setCode);

                                set = this.mMtgStore.GetSetByCode(setCode).Result;

                                if (set == null)
                                {
                                    this.mLoggingService.Warning("Set named '{0}' [{1}] has no set in the database with that code...", name, setCode);

                                    continue;
                                }
                                
                            }

                            UpdatePricesInSet(setUrl, set, setCode, name);
                        }
                        else
                        {
                            this.mLoggingService.Warning("Unable to parse details for set '{0}'...", setNameFull);
                        }
                    }
                }

            }
        }

        private void UpdatePricesInSet(string url, Set set, string setCodeAlternate, string setNameAlternate)
        {
            if (string.IsNullOrEmpty(url))
            {
                this.mLoggingService.Warning("Url is empty.");

                return;
            }

            if (set == null)
            {
                this.mLoggingService.Warning("set is null.");

                return;
            }

            string pageSource = this.mHttpClient.GetPageSource(url);
            if (string.IsNullOrEmpty(pageSource))
            {
                this.mLoggingService.Warning("Url '{0}' returned no data.", url);

                return;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            // Create set price
            HtmlNode setPrices = htmlDoc.DocumentNode.SelectSingleNode(@"//*[contains(@class,'todaysprices')]");

            if (setPrices != null)
            {
                SetPrice setPrice = new SetPrice();

                setPrice.Name = set.Name;
                setPrice.SetCode = set.Code;

                // Set alternate code and name
                if (set.Code.ToLower() != setCodeAlternate.ToLower())
                {
                    setPrice.SetCodeAlternate = setCodeAlternate;
                }

                string searchNameAlternate = this.mSearchUtility.GetSearchValue(setNameAlternate);
                if (set.SearchName != searchNameAlternate)
                {
                    setPrice.SearchNameAlternate = searchNameAlternate;
                }

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

                this.mLoggingService.Debug("Saved price for set '{0}'.", newSetPrice.Name);
            }

            HtmlNode cardsNode = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div[4]/div/div[5]/table/tbody");

            this.mLoggingService.Debug("Parsed cards node");

            if (cardsNode == null)
            {
                this.mLoggingService.Warning("CardsNode is NULL for '{0}'. Trying another method...", url);

                cardsNode = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id='set-table']/tbody");

                if (cardsNode == null)
                {
                    this.mLoggingService.Warning("CardsNode is _STILL_ NULL for '{0}'. Skipping...", url);

                    return;
                }
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
                    this.mLoggingService.Warning("Row is NULL");
                }

                HtmlNode nameNode = row.SelectSingleNode("td[3]");
                if (nameNode == null)
                {
                    this.mLoggingService.Warning("nameNode is NULL");
                }

                HtmlNode diffNode = row.SelectSingleNode("td[4]");
                if (diffNode == null)
                {
                    this.mLoggingService.Warning("diffNode is NULL");
                }

                HtmlNode lowNode = row.SelectSingleNode("td[5]");
                if (lowNode == null)
                {
                    this.mLoggingService.Warning("lowNode is NULL");
                }

                HtmlNode foilNode = row.SelectSingleNode("td[6]");
                if (nameNode == null)
                {
                    this.mLoggingService.Warning("foilNode is NULL");
                }

                // Skip cards that have the pattern (0-9) in the name (e.g. Plains (310)) since this are lands
                if (Regex.IsMatch(nameNode.InnerText, @"\([0-9]+\)"))
                {
                    this.mLoggingService.Warning("Skipping card '{0}' due to invalid pattern.", nameNode.InnerText);

                    continue;
                }

                CardPrice price = new CardPrice();
                price.SetCode = set.Code;

                // Set alternate Set code
                if (set.Code.ToLower() != setCodeAlternate.ToLower())
                {
                    price.SetCodeAlternate = setCodeAlternate;
                }

                price.Name = nameNode.InnerText;
                price.SearchName = this.mSearchUtility.GetSearchValue(price.Name);

                price.PriceDiff = diffNode.InnerText;
                price.PriceDiffValue = 0;

                this.mLoggingService.Debug("Card={0}; Set={1}; PriceDiff={2}; PriceDiffVal={3}",
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

                this.mLoggingService.Debug("TempPrice={0}", tempPrice);

                if (string.IsNullOrEmpty(tempPrice))
                {
                    this.mLoggingService.Debug("TempPrice is NULL");
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

                this.mLoggingService.Debug("PriceFoil={0}; PriceLow={1}; PriceMid={2}",
                    price.PriceFoil, price.PriceLow, price.PriceMid);

                string msg = string.Format("Inserting '{0}' from '{1}'... ",
                    price.Name,
                    price.SetCode);

                Console.WriteLine(msg);
                this.mLoggingService.Debug(msg);

                CardPrice card = this.mPriceStore.FindAndModifyCardPrice(price, true);

                this.mLoggingService.Debug("Saved price for card '{0}'.", card.Name);
            }
        }

        public void PurgePrices(DateTime date)
        {
            int card_prices_removed = this.mPriceStore.RemoveCardPricesOnOrBefore(date);
            int set_prices_removed = this.mPriceStore.RemoveSetPricesOnOrBefore(date);
        }
    }
}
