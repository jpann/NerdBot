using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleLogging.Core;

namespace NerdBot.Statistics
{
    public class QueryStatisticsStore : IQueryStatisticsStore
    {
        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;

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
            this.mServer = this.mClient.GetServer();
            this.mDatabase = this.mServer.GetDatabase(this.mDatabaseName);
            this.mLoggingService = loggingService;
        }

        public async Task<bool> InsertCardQueryStat(CardQueryStat stat)
        {
            if (stat == null)
                throw new ArgumentException("stat");

            var collection = this.mDatabase.GetCollection<CardQueryStat>("card_query_stats");

            collection.Save(stat);

            return true;
        }
    }
}
