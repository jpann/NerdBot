using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleLogging.Core;

namespace NerdBotCommon.Statistics
{
    public class QueryStatisticsStore : IQueryStatisticsStore
    {
        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;

        private const string cCardQueryCollection = "card_query_stats";

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

        public async Task<bool> InsertCardQueryStat(string userName, int userId, int multiverseId)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            CardQueryStat stat = new CardQueryStat()
            {
                UserId = userId,
                UserName = userName,
                MultiverseId = multiverseId,
                Date = DateTime.Now
            };

            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

            collection.Save(stat);

            return true;
        }

        public async Task<CardQueryStatData> GetCardQueryStatByMultiverseId(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<CardQueryStat>(cCardQueryCollection);

         //   var match = new BsonDocument
	        //{
		       // {
			      //  "$match", new BsonDocument
			      //  {
				     //   { "multiverseId", multiverseId }
			      //  }
		       // }
	        //};

         //   var group = new BsonDocument 
	        //{ 
		       // { 
			      //  "$group", new BsonDocument
			      //  {
				     //   {
					    //    "_id", new BsonDocument
					    //    {
						   //     { "userName", "$userName" }
					    //    }
				     //   },
				     //   {
					    //    "Count", new BsonDocument
					    //    {
						   //     { "$sum", 1 }
					    //    }
				     //   }
			      //  }
		       // } 
	        //};

         //   var sort = new BsonDocument
	        //{
		       // {
			      //  "$sort", new BsonDocument
			      //  {
				     //   { "Count", -1 }
			      //  }
		       // }
	        //};

         //   var pipeline = new[] { match, group, sort };
         //   var result = collection.Aggregate(pipeline);

         //   var matches = result.ResultDocuments
         //       .Select(x => x.AsBsonDocument)
         //       .ToList();

         //   if (matches.Any())
         //   {
         //       CardQueryStatData stat = new CardQueryStatData()
         //       {
         //           UserName = matches[0].GetValue("_id")["userName"].AsString,
         //           Count = matches[0].GetValue("Count").ToInt32(),
         //           MultiverseId = multiverseId
         //       };

         //       return stat;
         //   }

            return null;
        }

        public async Task<CardQueryStatData> GetCardQueryStatByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CardQueryStatData> GetCardQueryStatByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            throw new NotImplementedException();
        }
    }
}
