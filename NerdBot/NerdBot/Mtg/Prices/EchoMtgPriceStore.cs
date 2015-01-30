using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SimpleLogging.Core;

namespace NerdBot.Mtg.Prices
{
    public class EchoMtgPriceStore : ICardPriceStore
    {
        private const string cPriceCollectionName = "echo_prices";

        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;

        public EchoMtgPriceStore(
            string connectionString,
            string databaseName,
            ILoggingService loggingService)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("databaseName");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mConnectionString = connectionString;
            this.mDatabaseName = databaseName;
            this.mClient = new MongoClient(this.mConnectionString);
            this.mServer = this.mClient.GetServer();
            this.mDatabase = this.mServer.GetDatabase(this.mDatabaseName);
            this.mLoggingService = loggingService;
        }

        public string GetSearchValue(string text, bool forRegex = false)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            if (forRegex)
            {
                // Replace * and % with a regex '*' char
                searchValue = searchValue.Replace("%", ".*");

                // If the first character of the searchValue is not '*', 
                // meaning the user does not want to do a contains search,
                // explicitly use a starts with regex
                if (!searchValue.StartsWith(".*"))
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

        public CardPrice GetCardPrice(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            string searchName = this.GetSearchValue(name, true);

            this.mLoggingService.Debug("Getting price for '{0}' using search name '{1}'...",
                name,
                searchName);

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cPriceCollectionName);

            var query = Query<CardPrice>.Matches(c => c.SearchName, new BsonRegularExpression(searchName, "i"));
            var sortBy = SortBy.Ascending("lastUpdated");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardPriceCollection.FindAs<CardPrice>(query)
                .SetSortOrder(sortBy)
                .SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card.FirstOrDefault();
        }

        public CardPrice GetCardPrice(string name, string setCode)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setCode))
                throw new ArgumentException("setCode");

            string searchName = this.GetSearchValue(name, true);

            this.mLoggingService.Debug("Getting price for '{0}' [{1}] using search name '{2}'...",
                name,
                setCode,
                searchName);

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cPriceCollectionName);

            var query = Query.And(
                Query<CardPrice>.Matches(e => e.SearchName, new BsonRegularExpression(searchName, "i")),
                Query<CardPrice>.Matches(e => e.SetCode, setCode));

            var sortBy = SortBy.Ascending("lastUpdated");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardPriceCollection.FindAs<CardPrice>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card.FirstOrDefault();
        }

        public bool RemoveCardPrice(CardPrice cardPrice)
        {
            throw new NotImplementedException();
        }

        public CardPrice FindAndModifyCardPrice(CardPrice cardPrice, bool upsert = true)
        {
            if (cardPrice == null)
                throw new ArgumentNullException("cardPrice");

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cPriceCollectionName);

            // Query for this card
            var cardQuery = Query.And(
                Query.EQ("name", cardPrice.Name),
                Query.EQ("setCode", cardPrice.SetCode)
                );

            var cardSoryBy = SortBy.Descending("name");

            // Update document
            var cardUpdate = Update
                .Set("url", cardPrice.Url)
                .Set("name", cardPrice.Name)
                .Set("searchName", cardPrice.SearchName)
                .Set("lastUpdated", cardPrice.LastUpdated)
                .Set("setCode", cardPrice.SetCode)
                .Set("priceDiff", cardPrice.PriceDiff)
                .Set("priceMid", cardPrice.PriceMid)
                .Set("priceLow", cardPrice.PriceLow)
                .Set("priceFoil", cardPrice.PriceFoil);

            // Find and modify document. If document doesnt exist, insert it
            FindAndModifyArgs findModifyArgs = new FindAndModifyArgs();
            findModifyArgs.SortBy = cardSoryBy;
            findModifyArgs.Query = cardQuery;
            findModifyArgs.Upsert = true;
            findModifyArgs.Update = cardUpdate;
            findModifyArgs.VersionReturned = FindAndModifyDocumentVersion.Modified;

            var cardResult = cardPriceCollection.FindAndModify(findModifyArgs);
            var cardModified = cardResult.GetModifiedDocumentAs<CardPrice>();

            return cardModified;
        }

        public int RemoveCardPricesOnOrBefore(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
