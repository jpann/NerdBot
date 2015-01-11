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

            var card = collection.FindOne(query);

            return card;
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

            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i"))
                );

            var card = collection.FindOne(query);

            return card;
        }
        #endregion

        #region GetCards
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

            name = this.GetSearchValue(name, true);

            var collection = this.mDatabase.GetCollection<Card>("cards");

            var query = Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i"));

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

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

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("multiverseId");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            if (cards.Any())
            {
                Random rand = new Random();

                Card card = cards[rand.Next(cards.Count)];

                if (card != null)
                    return card;
            }

            return null;
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

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("name");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

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
        #endregion

        #region GetSet
        public async Task<Set> GetSet(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.GetSearchValue(name, false);

            var collection = this.mDatabase.GetCollection<Set>("sets");

            var query = Query<Set>.EQ(e => e.SearchName, name);

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
        #endregion
    }
}
