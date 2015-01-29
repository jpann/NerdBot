using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SimpleLogging.Core;

namespace NerdBot.Mtg
{
    public class MtgStore : IMtgStore
    {
        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;

        public MtgStore(
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

        #region CardExists
        public async Task<bool> CardExists(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.EQ(e => e.MultiverseId, multiverseId);

            long qty = collection.Count(query);

            if (qty > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> CardExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name, false);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            
            var query = Query<Card>.EQ(e => e.SearchName, name);
            long qty = collection.Count(query);

            if (qty > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> CardExists(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            name = this.GetSearchValue(name, false);
            setName = this.GetSearchValue(setName, false);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query.And(
                Query<Card>.EQ(e => e.SearchName, name),
                Query<Card>.EQ(e => e.SetSearchName, setName)
                );

            long qty = collection.Count(query);

            if (qty > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region GetCard
        public async Task<Card> GetCard(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name, true);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));
            var sortBy = SortBy.Ascending("setName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = collection.FindAs<Card>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card.FirstOrDefault();
        }

        public async Task<Card> GetCard(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            name = this.GetSearchValue(name, true);
            setName = this.GetSearchValue(setName, true);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            // Search both the set's search name and set id
            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query.Or(
                    Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                    Query<Card>.Matches(e => e.SetId, new BsonRegularExpression(setName, "i")))
                );

            var sortBy = SortBy.Ascending("setName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = collection.FindAs<Card>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();
            
            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card.FirstOrDefault();
        }

        #endregion

        #region GetCards
        public async Task<List<Card>> GetCards()
        {
            List<Card> cards = new List<Card>();

            var collection = this.mDatabase.GetCollection<Card>("cards");
            
            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor <Card> cursor = collection.FindAll()
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }

        public async Task<List<Card>> GetCards(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            List<Card> cards = new List<Card>();

            name = this.GetSearchValue(name, true);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var query = Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }
        #endregion

        #region GetRandomCards
        public async Task<Card> GetRandomCardByArtist(string artist)
        {
            if (string.IsNullOrEmpty(artist))
                throw new ArgumentException("artist");

            List<Card> cards = new List<Card>();

            var collection = this.mDatabase.GetCollection<Card>("cards");

            artist = artist.ToLower();

            // Replace * and % with a regex '.' char
            artist = artist.Replace("*", ".");
            artist = artist.Replace("%", ".");

            if (!artist.StartsWith("."))
            {
                artist = "^" + artist;
            }

            var query = Query<Card>.Matches(e => e.Artist, new BsonRegularExpression(artist, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Find(query).Count();

            var rand = new Random();
            var r = rand.Next(count);
            var card = collection.Find(query).Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }

        public async Task<Card> GetRandomCardInSet(string setName)
        {
            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            var collection = this.mDatabase.GetCollection<Card>("cards");

            setName = this.GetSearchValue(setName, false);

            var query = Query.Or(
                    Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                    Query<Card>.Matches(e => e.SetId, new BsonRegularExpression(setName, "i")));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int) collection.Find(query).Count();

            var rand = new Random();
            var r = rand.Next(count);
            var card = collection.Find(query).Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }

        public async Task<Card> GetRandomCard()
        {
            var collection = this.mDatabase.GetCollection<Card>("cards");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Count();
            var random = new Random();
            var r = random.Next(count);
            var card = collection.FindAll().Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }

        public async Task<Card> GetRandomCardWithStaticAbility(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("text");

            text = text.Replace("%", ".*");

            if (!text.EndsWith(".*"))
                text = text + ".*";

            text = "^" + text;

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.Matches(e => e.Desc, new BsonRegularExpression(text, "i"));
            var sortBy = SortBy.Ascending("multiverseId");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Find(query).Count();

            var rand = new Random();
            var r = rand.Next(count);
            var card = collection.Find(query).SetSortOrder(sortBy).Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }
        #endregion

        #region GetCardSets
        public async Task<List<Set>> GetCardSets(int multiverseId, int limit = 8)
        {
            List<Set> sets = new List<Set>();

            var cardsCollection = this.mDatabase.GetCollection<Card>("cards");
            var setsCollection = this.mDatabase.GetCollection<Set>("sets");

            // Query to get the card with this multiverseId
            var cardMultiverseIdQuery = Query<Card>.EQ(e => e.MultiverseId, multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.FindOne(cardMultiverseIdQuery);

            if (card != null)
            {
                // Queries to get all other cards that share the card's name
                var cardNameQuery = Query<Card>.EQ(e => e.SearchName, card.SearchName);

                MongoCursor<Card> cursor = cardsCollection.Find(cardNameQuery)
                    .SetSortOrder("setName");

                List<IMongoQuery> setQueries = new List<IMongoQuery>();

                // Go throug each card that shares this name and add create a set query for it
                foreach (Card c in cursor)
                {
                    setQueries.Add(Query<Set>.EQ(e => e.SearchName, c.SetSearchName));
                }

                // Get all sets for cards that share this name
                MongoCursor<Set> setCursor = setsCollection.Find(Query.Or(setQueries))
                    .SetSortOrder("name")
                    .SetLimit(limit);

                foreach (Set s in setCursor)
                {
                    sets.Add(s);
                }
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return sets;
        }
        #endregion

        #region GetCardOtherSets
        public async Task<List<Set>> GetCardOtherSets(int multiverseId)
        {
            List<Set> sets = new List<Set>();

            var cardsCollection = this.mDatabase.GetCollection<Card>("cards");
            var setsCollection = this.mDatabase.GetCollection<Set>("sets");

            // Query to get the card with this multiverseId
            var cardMultiverseIdQuery = Query<Card>.EQ(e => e.MultiverseId, multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.FindOne(cardMultiverseIdQuery);

            if (card != null)
            {
                // Queries to get all other cards that do not have this multiverseId but share the card's name
                var cardNotMultiverseIdQuery = Query<Card>.NE(e => e.MultiverseId, multiverseId);
                var cardNameQuery = Query<Card>.EQ(e => e.SearchName, card.SearchName);

                var cardOtherQuery = Query.And(
                    cardNotMultiverseIdQuery,
                    cardNameQuery
                    );

                MongoCursor<Card> cursor = cardsCollection.Find(cardOtherQuery)
                    .SetSortOrder("name");

                List<IMongoQuery> setQueries = new List<IMongoQuery>();

                // Go throug each card that shares this name and add create a set query for it
                foreach (Card c in cursor)
                {
                    setQueries.Add(Query<Set>.EQ(e => e.SearchName, c.SetSearchName));
                }

                if (setQueries.Any())
                {
                    // Get all sets for cards that share this name
                    MongoCursor<Set> setCursor = setsCollection.Find(Query.Or(setQueries))
                        .SetSortOrder("name");

                    foreach (Set s in setCursor)
                    {
                        sets.Add(s);
                    }
                }
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return sets;
        }
        #endregion

        #region GetCardsBySet
        public async Task<List<Card>> GetCardsBySet(string setName)
        {
            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            List<Card> cards = new List<Card>();

            setName = this.GetSearchValue(setName, false);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.EQ(e => e.SetSearchName, setName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }
        #endregion

        #region SetExists
        public async Task<bool> SetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name, false);

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.EQ(e => e.SearchName, name);
     
            Stopwatch watch = new Stopwatch();
            watch.Start();

            long qty = collection.Count(query);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            if (qty > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> SetExistsByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("code");

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.EQ(e => e.Code, code);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            long qty = collection.Count(query);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            if (qty > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region GetSet
        public async Task<Set> GetSet(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name, true);

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var set = collection.FindOne(query);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return set;
        }

        public async Task<Set> GetSetByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("code");

            var collection = this.mDatabase.GetCollection<Set>("sets");

            // BsonRegEx in order to do a case insensitive match
            var query = Query<Set>.Matches(e => e.Code, new BsonRegularExpression(code, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var sets = collection.FindOne(query);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return sets;
        }
        #endregion
    }
}
