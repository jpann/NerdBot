using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public async Task<List<Card>> GetCards(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Set>> GetCardOtherSets(int multiverseId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Card>> GetCardsBySet(string setName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetExists(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetExistsByCode(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<Set> GetSet(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<Set> GetSetByCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
