using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NerdBotCommon.Utilities;
using SimpleLogging.Core;

namespace NerdBotCommon.Mtg.Prices
{
    public class EchoMtgPriceStore : ICardPriceStore
    {
        private const string cCardPriceCollectionName = "echo_prices";
        private const string cSetPriceCollectionName = "echo_set_prices";

        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;
        private readonly SearchUtility mSearchUtility;

        public EchoMtgPriceStore(
            string connectionString,
            string databaseName,
            ILoggingService loggingService,
            SearchUtility searchUtility)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("databaseName");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            if (searchUtility == null)
                throw new ArgumentNullException("searchUtility");

            this.mConnectionString = connectionString;
            this.mDatabaseName = databaseName;
            this.mClient = new MongoClient(this.mConnectionString);
            this.mServer = this.mClient.GetServer();
            this.mDatabase = this.mServer.GetDatabase(this.mDatabaseName);
            this.mLoggingService = loggingService;
            this.mSearchUtility = searchUtility;
        }

        #region Sets

        public SetPrice GetSetPriceByCode(string code)
        {
            this.mLoggingService.Debug("Getting set price for '{0}' using search code...",
                code);

            var setPriceCollection = this.mDatabase.GetCollection<SetPrice>(cSetPriceCollectionName);

            var query = Query<SetPrice>.EQ(e => e.SetCode, code);
            var sortBy = SortBy.Ascending("lastUpdated");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var set = setPriceCollection.FindAs<SetPrice>(query)
                .SetSortOrder(sortBy)
                .SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return set.FirstOrDefault();
        }

        public SetPrice GetSetPrice(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            string searchName = this.mSearchUtility.GetRegexSearchValue(name);

            this.mLoggingService.Debug("Getting set price for '{0}' using search name '{1}'...",
                name,
                searchName);

            var setPriceCollection = this.mDatabase.GetCollection<SetPrice>(cSetPriceCollectionName);

            var query = Query<SetPrice>.Matches(c => c.SearchName, new BsonRegularExpression(searchName, "i"));
            var sortBy = SortBy.Ascending("lastUpdated");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var set = setPriceCollection.FindAs<SetPrice>(query)
                .SetSortOrder(sortBy)
                .SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return set.FirstOrDefault();
        }

        public bool RemoveSetPrice(SetPrice setPrice)
        {
            if (setPrice == null)
                throw new ArgumentNullException("setPrice");

            var setPriceCollection = this.mDatabase.GetCollection<SetPrice>(cSetPriceCollectionName);

            var query = Query<SetPrice>.EQ(c => c.Id, setPrice.Id);

            var removeResult = setPriceCollection.Remove(query);

            if (removeResult.Ok)
                return true;
            else
                return false;
        }

        public int RemoveSetPricesOnOrBefore(DateTime date)
        {
            if (date == DateTime.MinValue)
                throw new ArgumentException("date");

            var setPriceCollection = this.mDatabase.GetCollection<CardPrice>(cSetPriceCollectionName);

            var query = Query<SetPrice>.LTE(c => c.LastUpdated, date.ToUniversalTime());

            var removeResult = setPriceCollection.Remove(query);

            return (int)removeResult.DocumentsAffected;
        }

        public SetPrice FindAndModifySetPrice(SetPrice setPrice, bool upsert = true)
        {
            if (setPrice == null)
                throw new ArgumentNullException("setPrice");

            var setPriceCollection = this.mDatabase.GetCollection<CardPrice>(cSetPriceCollectionName);

            // Query for this set
            var setQuery = Query.And(
                Query.EQ("name", setPrice.Name),
                Query.EQ("setCode", setPrice.SetCode)
            );

            var cardSoryBy = SortBy.Descending("name");

            // Update document
            var cardUpdate = Update
                .Set("url", setPrice.Url)
                .Set("name", setPrice.Name)
                .Set("searchName", setPrice.SearchName)
                .Set("lastUpdated", setPrice.LastUpdated)
                .Set("setCode", setPrice.SetCode)
                .Set("totalCards", setPrice.TotalCards)
                .Set("setValue", setPrice.SetValue)
                .Set("foilSetValue", setPrice.FoilSetValue);

            // Find and modify document. If document doesnt exist, insert it
            FindAndModifyArgs findModifyArgs = new FindAndModifyArgs();
            findModifyArgs.SortBy = cardSoryBy;
            findModifyArgs.Query = setQuery;
            findModifyArgs.Upsert = true;
            findModifyArgs.Update = cardUpdate;
            findModifyArgs.VersionReturned = FindAndModifyDocumentVersion.Modified;

            var setResult = setPriceCollection.FindAndModify(findModifyArgs);
            var setModified = setResult.GetModifiedDocumentAs<SetPrice>();

            return setModified;
        }
        #endregion

        #region Cards
        public CardPrice GetCardPrice(int multiverseId)
        {
            this.mLoggingService.Debug("Getting price for '{0}' using search multiverseId...",
                multiverseId);

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            var query = Query<CardPrice>.EQ(e => e.MultiverseId, multiverseId);
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

        public CardPrice GetCardPrice(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            string searchName = this.mSearchUtility.GetRegexSearchValue(name);

            this.mLoggingService.Debug("Getting price for '{0}' using search name '{1}'...",
                name,
                searchName);

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

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

            string searchName = this.mSearchUtility.GetRegexSearchValue(name);

            this.mLoggingService.Debug("Getting price for '{0}' [{1}] using search name '{2}'...",
                name,
                setCode,
                searchName);

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

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
            if (cardPrice == null)
                throw new ArgumentNullException("cardPrice");

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            var query = Query<CardPrice>.EQ(c => c.Id, cardPrice.Id);

            var removeResult = cardPriceCollection.Remove(query);

            if (removeResult.Ok)
                return true;
            else
                return false;
        }

        public int RemoveCardPricesOnOrBefore(DateTime date)
        {
            if (date == DateTime.MinValue)
                throw new ArgumentException("date");

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            var query = Query<CardPrice>.LTE(c => c.LastUpdated, date.ToUniversalTime());

            var removeResult = cardPriceCollection.Remove(query);

            return (int)removeResult.DocumentsAffected;
        }

        public CardPrice FindAndModifyCardPrice(CardPrice cardPrice, bool upsert = true)
        {
            if (cardPrice == null)
                throw new ArgumentNullException("cardPrice");

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            // Query for this card
            var cardQuery = Query.And(
                Query.EQ("name", cardPrice.Name),
                Query.EQ("setCode", cardPrice.SetCode),
                Query.EQ("multiverseId", cardPrice.MultiverseId)
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
                .Set("priceDiffValue", cardPrice.PriceDiffValue)
                .Set("priceMid", cardPrice.PriceMid)
                .Set("priceLow", cardPrice.PriceLow)
                .Set("priceFoil", cardPrice.PriceFoil)
                .Set("multiverseId", cardPrice.MultiverseId)
                .Set("imageUrl", cardPrice.ImageUrl);

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

        public List<CardPrice> GetCardsByPriceIncrease(int limit = 10)
        {
            List<CardPrice> prices = new List<CardPrice>();

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            var cardQuery = Query.And(
                Query.GT("priceDiffValue", 0),
                Query.NE("priceMid", ""),
                Query.NE("priceLow", ""),
                Query.NE("priceFoil", ""),
                Query.NE("searchName", "swamp"),
                Query.NE("searchName", "mountain"),
                Query.NE("searchName", "plains"),
                Query.NE("searchName", "island"),
                Query.NE("searchName", "forest")
                );

            var sortBy = SortBy.Descending("priceDiffValue");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor<CardPrice> cursor = cardPriceCollection.FindAs<CardPrice>(cardQuery)
                .SetSortOrder(sortBy)
                .SetLimit(limit);

            foreach (CardPrice price in cursor)
            {
                prices.Add(price);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return prices;
        }

        public List<CardPrice> GetCardsByPriceDecrease(int limit = 10)
        {
            List<CardPrice> prices = new List<CardPrice>();

            var cardPriceCollection = this.mDatabase.GetCollection<CardPrice>(cCardPriceCollectionName);

            var cardQuery = Query.And(
                Query.LT("priceDiffValue", 0),
                Query.NE("priceMid", ""),
                Query.NE("priceLow", ""),
                Query.NE("priceFoil", ""),
                Query.NE("searchName", "swamp"),
                Query.NE("searchName", "mountain"),
                Query.NE("searchName", "plains"),
                Query.NE("searchName", "island"),
                Query.NE("searchName", "forest")
                );

            var sortBy = SortBy.Ascending("priceDiffValue");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor<CardPrice> cursor = cardPriceCollection.FindAs<CardPrice>(cardQuery)
                .SetSortOrder(sortBy)
                .SetLimit(limit);

            foreach (CardPrice price in cursor)
            {
                prices.Add(price);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return prices;
        }
        #endregion
    }
}
