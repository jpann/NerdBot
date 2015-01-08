using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NerdBot.Mtg
{
    public class MtgStore : IMtgStore
    {
        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;

        public MtgStore(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("databaseName");

            this.mConnectionString = connectionString;
            this.mDatabaseName = databaseName;
            this.mClient = new MongoClient(this.mConnectionString);
            this.mServer = this.mClient.GetServer();
            this.mDatabase = this.mServer.GetDatabase(this.mDatabaseName);
        }

        public string GetSearchValue(string text)
        {
            string searchValue = text.ToLower();

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            searchValue = rgx.Replace(searchValue, "");
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

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

            name = this.GetSearchValue(name);

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

            name = this.GetSearchValue(name);
            setName = this.GetSearchValue(setName);

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

        public async Task<Card> GetCard(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));

            var card = collection.FindOne(query);

            return card;
        }

        public async Task<Card> GetCard(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            name = this.GetSearchValue(name);
            setName = this.GetSearchValue(setName);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i"))
                );

            var card = collection.FindOne(query);

            return card;
        }

        public async Task<List<Card>> GetCards()
        {
            List<Card> cards = new List<Card>();

            var collection = this.mDatabase.GetCollection<Card>("cards");
            
            MongoCursor <Card> cursor = collection.FindAll()
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            return cards;
        }

        public async Task<List<Card>> GetCards(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            List<Card> cards = new List<Card>();

            name = this.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.EQ(e => e.SearchName, name);

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            return cards;
        }

        public async Task<List<Set>> GetCardOtherSets(int multiverseId)
        {
            List<Set> sets = new List<Set>();

            var cardsCollection = this.mDatabase.GetCollection<Card>("cards");
            var setsCollection = this.mDatabase.GetCollection<Set>("sets");

            // Query to get the card with this multiverseId
            var cardMultiverseIdQuery = Query<Card>.EQ(e => e.MultiverseId, multiverseId);

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

                // Get all sets for cards that share this name
                MongoCursor<Set> setCursor = setsCollection.Find(Query.Or(setQueries))
                    .SetSortOrder("name");

                foreach (Set s in setCursor)
                {
                    sets.Add(s);
                }
            }

            return sets;
        }

        public async Task<List<Card>> GetCardsBySet(string setName)
        {
            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            List<Card> cards = new List<Card>();

            setName = this.GetSearchValue(setName);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.EQ(e => e.SetSearchName, setName);

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            return cards;
        }

        public async Task<bool> SetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.EQ(e => e.SearchName, name);

            long qty = collection.Count(query);

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

            long qty = collection.Count(query);

            if (qty > 0)
                return true;
            else
                return false;
        }

        public async Task<Set> GetSet(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));

            var set = collection.FindOne(query);

            return set;
        }

        public async Task<Set> GetSetByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("code");

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.EQ(e => e.Code, code);

            var sets = collection.FindOne(query);

            return sets;
        }
    }
}
