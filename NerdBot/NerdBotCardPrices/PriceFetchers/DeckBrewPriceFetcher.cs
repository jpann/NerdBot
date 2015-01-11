using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdBot.Http;
using Newtonsoft.Json;

namespace NerdBotCardPrices.PriceFetchers
{
    public class DeckBrewPriceFetcher
    {
        private readonly string mUrl;
        private readonly IHttpClient mHttpClient;

        public DeckBrewPriceFetcher(string url, IHttpClient httpClient)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");
            
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            this.mUrl = url;
            this.mHttpClient = httpClient;
        }


        //TODO This is lame
        public string[] GetPrice(int multiverseId)
        {
            // Build the request url for this card
            string url = string.Format("{0}/cards?multiverseid={1}", this.mUrl, multiverseId);

            string jsonData = this.mHttpClient.GetAsJson(url).Result;

            if (string.IsNullOrEmpty(jsonData))
                return null;

            List<DeckBrewCard> card = JsonConvert.DeserializeObject<List<DeckBrewCard>>(jsonData);
            if (!card.Any())
                return null;

            if (!card[0].Editions.Any())
                return null;

            foreach (DeckBrewEdition edition in card[0].Editions)
            {
                if (edition.Price == null)
                    continue;

                double high = edition.Price.High;
                double median = edition.Price.Median;
                double low = edition.Price.Low;
                string storeUrl = edition.Store_Url;

                return new string[]
                {
                    low.ToString("0.00"),
                    median.ToString("0.00"), 
                    high.ToString("0.00"), 
                    storeUrl
                };
            }

            return null;
        }
    }

    public class DeckBrewCard
    {
        public List<DeckBrewEdition> Editions { get; set; }
    }

    public class DeckBrewEdition
    {
        public int Multiverse_Id { get; set; }
        public DeckBrewCardPrice Price { get; set; }
        public string Store_Url { get; set; }
    }

    public class DeckBrewCardPrice
    {
        private double mHigh;
        private double mMedian;
        private double mLow;

        public double High
        {
            get { return this.mHigh; }
            set { this.mHigh = value / 100; }
        }

        public double Median
        {
            get { return this.mMedian; }
            set { this.mMedian = value / 100; }
        }

        public double Low
        {
            get { return this.mLow; }
            set { this.mLow = value / 100; }
        }
    }
}
