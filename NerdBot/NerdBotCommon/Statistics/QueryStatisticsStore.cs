using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleLogging.Core;

namespace NerdBotCommon.Statistics
{
    public class QueryStatisticsStore : IQueryStatisticsStore
    {
        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly IMongoClient mClient;
        private readonly IMongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;

        private const string cCardQueryCollection = "card_query_stats";
        private const string cQueryCollection = "query_stats";

        public QueryStatisticsStore(
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
            this.mDatabase = this.mClient.GetDatabase(this.mDatabaseName);
            this.mLoggingService = loggingService;
        }

        public async Task<bool> InsertCardQueryStat(string userName, string userId, int multiverseId, string searchTerm)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            CardQueryStat stat = new CardQueryStat()
            {
                UserId = userId,
                UserName = userName,
                MultiverseId = multiverseId,
                SearchTerm = searchTerm,
                Date = DateTime.Now
            };

            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            collection.InsertOne(stat);

            return true;
        }

        public async Task<List<CardQueryStat>> GetCardQueryStatsByMultiverseId(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            List<CardQueryStat> stats = new List<CardQueryStat>();

            var query = Builders<CardQueryStat>.Filter.Eq("multiverseId", multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cursor = collection.Find(query)
                .Sort(Builders<CardQueryStat>.Sort.Ascending("dateTime")).ToCursor();

            foreach (CardQueryStat stat in cursor.ToEnumerable())
            {
                stats.Add(stat);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }

        public async Task<List<CardQueryStat>> GetCardQueryStatsByUserId(string userId)
        {
            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            List<CardQueryStat> stats = new List<CardQueryStat>();

            var query = Builders<CardQueryStat>.Filter.Eq("userId", userId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cursor = collection.Find(query)
                .Sort(Builders<CardQueryStat>.Sort.Ascending("dateTime")).ToCursor();

            foreach (CardQueryStat stat in cursor.ToEnumerable())
            {
                stats.Add(stat);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }

        public async Task<List<CardQueryStat>> GetCardQueryStatsByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            List<CardQueryStat> stats = new List<CardQueryStat>();

            var query = Builders<CardQueryStat>.Filter.Eq("userName", userName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cursor = collection.Find(query)
                .Sort(Builders<CardQueryStat>.Sort.Ascending("dateTime")).ToCursor();

            foreach (CardQueryStat stat in cursor.ToEnumerable())
            {
                stats.Add(stat);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }

        public async Task<List<CardQueryStat>> GetCardQueryStatsBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException("searchTerm");

            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            List<CardQueryStat> stats = new List<CardQueryStat>();


            Stopwatch watch = new Stopwatch();
            watch.Start();

            var filter = Builders<CardQueryStat>.Filter.Text(searchTerm);
            var projection = Builders<CardQueryStat>.Projection.MetaTextScore("TextMatchScore");
            var sort = Builders<CardQueryStat>.Sort.MetaTextScore("TextMatchScore");

            stats = collection.Find(filter)
                .Project<CardQueryStat>(projection)
                .Sort(sort)
                .ToList();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }

        public async void InsertQueryStat(string searchTerm, int multiverseId)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException("searchTerm");

            QueryStat stat = new QueryStat()
            {
                MultiverseId = multiverseId,
                SearchTerm = searchTerm,
                Date = DateTime.Now
            };

            var collection = this.mDatabase.GetCollection<QueryStat>(cQueryCollection);

            collection.InsertOne(stat);
        }

        public async Task<List<QueryStat>> GetQueryStatsBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException("searchTerm");

            var collection = this.mDatabase.GetCollection<QueryStat>(cQueryCollection);

            List<QueryStat> stats = new List<QueryStat>();


            Stopwatch watch = new Stopwatch();
            watch.Start();

            var filter = Builders<QueryStat>.Filter.Text(searchTerm);
            var projection = Builders<QueryStat>.Projection.MetaTextScore("TextMatchScore");
            var sort = Builders<QueryStat>.Sort.MetaTextScore("TextMatchScore");

            stats = collection.Find(filter)
                .Project<QueryStat>(projection)
                .Sort(sort)
                .ToList();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }

        public async Task<List<QueryStat>> GetQueryStatsByMultiverseId(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<QueryStat>(cQueryCollection);

            List<QueryStat> stats = new List<QueryStat>();

            var query = Builders<QueryStat>.Filter.Eq("multiverseId", multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cursor = collection.Find(query)
                .Sort(Builders<QueryStat>.Sort.Ascending("dateTime")).ToCursor();

            foreach (QueryStat stat in cursor.ToEnumerable())
            {
                stats.Add(stat);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return stats;
        }
    }
}
