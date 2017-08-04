using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NerdBot.Utilities;
using SimpleLogging.Core;

namespace NerdBot.Mtg
{
    public class MtgStore : IMtgStore
    {
        private const string cCardsCollectionName = "cards";
        private const string cSetsCollectionName = "sets";

        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly MongoClient mClient;
        private readonly MongoServer mServer;
        private readonly MongoDatabase mDatabase;
        private readonly ILoggingService mLoggingService;
        private readonly SearchUtility mSearchUtility;

        public MtgStore(
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

        #region AddCard
        public async Task<Card> AddCard(Card card)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var result = collection.Insert(card);

            if (result.Ok)
                return card;
            else
                return null;
        }

        public async Task<Card> CardFindAndModify(Card card)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            // Query for this card
            var cardQuery = Query.And(
                Query.EQ("name", card.Name),
                Query.EQ("setId", card.SetId),
                Query.EQ("multiverseId", card.MultiverseId)
                );

            var cardSoryBy = SortBy.Descending("name");

            // Create BsonDocumentWrapper for the card's Colors array
            var colorsDocument = BsonDocumentWrapper.CreateMultiple(card.Colors);
            var colorsArray = new BsonArray(colorsDocument);

            // Create BsonDocumentWrapper for the card's Rulings array
            var rulingsDocument = BsonDocumentWrapper.CreateMultiple(card.Rulings);
            var rulingArray = new BsonArray(rulingsDocument);

            var colorIdentityDocument = BsonDocumentWrapper.CreateMultiple(card.ColorIdentity);
            var colorIdentityArray = new BsonArray(colorIdentityDocument);

            var variationsDocument = BsonDocumentWrapper.CreateMultiple(card.Variations);
            var variationsArray = new BsonArray(variationsDocument);

            var typesDocument = BsonDocumentWrapper.CreateMultiple(card.Types);
            var typesArray = new BsonArray(typesDocument);


            // Update document
            var cardUpdate = Update
                .Set("relatedCardId", card.RelatedCardId)
                .Set("name", card.Name)
                .Set("searchName", card.SearchName)
                .Set("desc", card.Desc ?? "")
                .Set("flavor", card.Flavor ?? "")
                .Set("colors", colorsArray) // Use BsonArray that contains a BsonDocumentWrapper of the card's colors
                .Set("cost", card.Cost ?? "")
                .Set("cmc", card.Cmc)
                .Set("setName", card.SetName)
                .Set("setSearchName", card.SetSearchName)
                .Set("type", card.Type ?? "")
                .Set("subType", card.SubType ?? "")
                .Set("power", card.Power ?? "")
                .Set("toughness", card.Toughness ?? "")
                .Set("loyalty", card.Loyalty ?? "")
                .Set("rarity", card.Rarity ?? "")
                .Set("artist", card.Artist ?? "")
                .Set("setId", card.SetId)
                .Set("token", card.Token)
                .Set("rulings", rulingArray) // Use BsonArray that contains a BsonDocumentWrapper of the card's rulings
                .Set("img", card.Img ?? "")
                .Set("imgHires", card.ImgHires ?? "")
                .Set("multiverseId", card.MultiverseId)
                .Set("number", card.Number ?? "")
                .Set("mciNumber", card.McINumber ?? "")
                .Set("colorIdentity", colorIdentityArray)
                .Set("variations", variationsArray)
                .Set("types", typesArray);

            // Find and modify document. If document doesnt exist, insert it
            FindAndModifyArgs findModifyArgs = new FindAndModifyArgs();
            findModifyArgs.SortBy = cardSoryBy;
            findModifyArgs.Query = cardQuery;
            findModifyArgs.Upsert = true;
            findModifyArgs.Update = cardUpdate;
            findModifyArgs.VersionReturned = FindAndModifyDocumentVersion.Modified;

            var cardResult = collection.FindAndModify(findModifyArgs);
            var cardModified = cardResult.GetModifiedDocumentAs<Card>();

            return cardModified;
        }
        #endregion

        #region CardExists
        public async Task<bool> CardExists(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

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

            name = this.mSearchUtility.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            
            var query = Query.And(
                Query<Card>.EQ(e => e.SearchName, name),
                Query.NE("multiverseId", 0));

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

            name = this.mSearchUtility.GetSearchValue(name);
            setName = this.mSearchUtility.GetSearchValue(setName);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query.And(
                Query<Card>.EQ(e => e.SearchName, name),
                Query<Card>.EQ(e => e.SetSearchName, setName),
                Query.NE("multiverseId", 0)
                );

            long qty = collection.Count(query);

            if (qty > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region GetCard
        public async Task<Card> GetCard(int multiverseId)
        {
            this.mLoggingService.Trace("Getting card by id '{0}'...", multiverseId);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query<Card>.EQ(e => e.MultiverseId, multiverseId);
            var sortBy = SortBy.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.FindAs<Card>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            var card = cardResult.FirstOrDefault();

            if (card == null)
            {
                this.mLoggingService.Warning("No card found using '{0}'.", multiverseId);
            }
            else
            {
                this.mLoggingService.Trace("Card found using '{0}': {1} [{2}]", 
                    multiverseId, 
                    card.Name, 
                    card.SetName);
            }

            return card;
        }

        public async Task<Card> GetCard(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            this.mLoggingService.Trace("Getting card by name '{0}'...", name);

            name = this.mSearchUtility.GetRegexSearchValue(name);

            this.mLoggingService.Trace("Search name for '{0}'.", name);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query.NE("multiverseId", 0));

            var sortBy = SortBy.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.FindAs<Card>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            var card = cardResult.FirstOrDefault();

            if (card == null)
            {
                this.mLoggingService.Warning("No card found using '{0}'.", name);
            }
            else
            {
                this.mLoggingService.Trace("Card found using '{0}': {1} [{2}]",
                    name,
                    card.Name,
                    card.SetName);
            }

            return card;
        }

        public async Task<Card> GetCard(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            this.mLoggingService.Trace("Getting card by name '{0}' in set '{1}'...", name, setName);

            name = this.mSearchUtility.GetRegexSearchValue(name);
            string setCode = setName;
            setName = this.mSearchUtility.GetRegexSearchValue(setName);

            this.mLoggingService.Trace("Search name for '{0}' and set '{1}'.", name, setName);
            
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            // Search both the set's search name and set id
            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query.NE("multiverseId", 0),
                Query.Or(
                    Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                    Query<Card>.EQ(e => e.SetId, new BsonRegularExpression(setCode, "i")))
                );

            var sortBy = SortBy.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.FindAs<Card>(query).SetSortOrder(sortBy).SetLimit(1);

            watch.Stop();
            
            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            var card = cardResult.FirstOrDefault();

            if (card == null)
            {
                this.mLoggingService.Warning("No card found using '{0}' in '{1}'.", name, setName);
            }
            else
            {
                this.mLoggingService.Trace("Card found using '{0}' in '{1}': {2} [{3}]",
                    name,
                    setName,
                    card.Name,
                    card.SetName);
            }

            return card;
        }

        #endregion

        #region GetCards
        public async Task<List<Card>> GetCards()
        {
            List<Card> cards = new List<Card>();

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);
            
            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor <Card> cursor = collection.FindAll()
                .SetSortOrder("searchName");

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }

        public async Task<List<Card>> GetCards(string name, int limit = 0)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            List<Card> cards = new List<Card>();

            name = this.mSearchUtility.GetRegexSearchValue(name);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();
				                
            var query = Query.And(
                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Query.NE("multiverseId", 0));

            MongoCursor<Card> cursor = collection.Find(query)
                .SetSortOrder("searchName");

            if (limit > 0)
                cursor.SetLimit(limit);

            foreach (Card card in cursor)
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }

		public async Task<List<Card>> SearchCards(string name, int limit = 0)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name");

			List<Card> cards = new List<Card>();

			string regex_name = this.mSearchUtility.GetRegexSearchValue(name);

			var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

			Stopwatch watch = new Stopwatch();
			watch.Start();

			IMongoQuery query = null;
			MongoCursor<Card> cursor = null;

			string[] names = name.Split(' ');
			if (names.Length > 1)
			{
				List<IMongoQuery> nameQueries = new List<IMongoQuery>();

				foreach (string n in names)
				{
					nameQueries.Add(Query.And(
                        Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(n, "i")),
                        Query.NE("multiverseId", 0)));
				}

				cursor = collection.Find(Query.Or(nameQueries))
					.SetSortOrder("searchName");
			}
			else
			{
				query = Query.And(
                    Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(regex_name, "i")),
                    Query.NE("multiverseId", 0));

				cursor = collection.Find(query)
					.SetSortOrder("searchName");
			}

			if (limit > 0)
				cursor.SetLimit(limit);

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

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            artist = artist.ToLower();

            // Replace * and % with a regex '.' char
            artist = artist.Replace("*", ".");
            artist = artist.Replace("%", ".");

            if (!artist.StartsWith("."))
            {
                artist = "^" + artist;
            }

            var query = Query.And(
                Query<Card>.Matches(e => e.Artist, new BsonRegularExpression(artist, "i")),
                Query.NE("multiverseId", 0));

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

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            string setCode = setName;
            setName = this.mSearchUtility.GetSearchValue(setName);

            var query = Query.And(
                    Query.NE("multiverseId", 0),
                    Query.Or(
                    Query<Card>.Matches(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                    Query<Card>.EQ(e => e.SetId, new BsonRegularExpression(setCode, "i")))
                    );

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
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

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

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query.And(
                Query.NE("multiverseId", 0),
                Query<Card>.Matches(e => e.Desc, new BsonRegularExpression(text, "i")));

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

        public async Task<Card> GetRandomCardWithDescription(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("text");

            text = text.Replace("%", ".*");

            if (!text.EndsWith(".*"))
                text = text + ".*";

            text = ".*" + text;

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query.And(
                Query<Card>.Matches(e => e.Desc, new BsonRegularExpression(text, "i")),
                Query.NE("multiverseId", 0));

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

        public async Task<string> GetRandomFlavorText()
        {
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Count();
            var random = new Random();
            var r = random.Next(count);
            Card card = collection.FindAll().Skip(r).FirstOrDefault();

            if (card == null)
            {
                int maxTries = 5;
                int tries = 0;

                do
                {
                    var randomNum = random.Next(count);
                    card = collection.FindAll().Skip(randomNum).FirstOrDefault();
                } 
                while (card == null && tries < maxTries);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card.Flavor;
        }
        #endregion

        #region GetCardSets
        public async Task<List<Set>> GetCardSets(int multiverseId, int limit = 8)
        {
            List<Set> sets = new List<Set>();

            var cardsCollection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);
            var setsCollection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            // Query to get the card with this multiverseId
            var cardMultiverseIdQuery = Query<Card>.EQ(e => e.MultiverseId, multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.FindOne(cardMultiverseIdQuery);

            if (card != null)
            {
                // Queries to get all other cards that share the card's name
                var cardNameQuery = Query.And(
                    Query.NE("multiverseId", 0),
                    Query<Card>.EQ(e => e.SearchName, card.SearchName));

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

            var cardsCollection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);
            var setsCollection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            // Query to get the card with this multiverseId
            var cardMultiverseIdQuery = Query.And(
                Query.NE("multiverseId", 0),
                Query<Card>.EQ(e => e.MultiverseId, multiverseId));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.FindOne(cardMultiverseIdQuery);

            if (card != null)
            {
                // Queries to get all other cards that do not have this multiverseId but share the card's name
                var cardNotMultiverseIdQuery = Query.And(
                    Query.NE("multiverseId", 0),
                    Query<Card>.NE(e => e.MultiverseId, multiverseId));

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

            setName = this.mSearchUtility.GetSearchValue(setName);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Query.And(
                Query.NE("multiverseId", 0),
                Query<Card>.EQ(e => e.SetSearchName, setName));

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

        #region AddSet
        public async Task<Set> AddSet(Set set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            var result = collection.Insert(set);

            if (result.Ok)
                return set;
            else
                return null;
        }

        public async Task<Set> SetFindAndModify(Set set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            // Query for this set
            var cardQuery = Query.And(
                Query.EQ("name", set.Name),
                Query.EQ("code", set.Code)
                );

            var setSortBy = SortBy.Descending("name");

            // Update document
            var setUpdate = Update
                .Set("name", set.Name)
                .Set("searchName", set.SearchName)
                .Set("code", set.Code)
                .Set("block", set.Block ?? "")
                .Set("type", set.Type ?? "")
                .Set("desc", set.Desc ?? "")
                .Set("commonQty", set.CommonQty)
                .Set("uncommonQty", set.UncommonQty)
                .Set("rareQty", set.RareQty)
                .Set("mythicQty", set.MythicQty)
                .Set("basicLandQty", set.BasicLandQty)
                .Set("totalQty", set.TotalQty)
                .Set("releasedOn", set.ReleasedOn)
                .Set("gathererCode", set.GathererCode ?? "")
                .Set("oldCode", set.OldCode ?? "")
                .Set("magicCardsInfoCode", set.MagicCardsInfoCode ?? "");

            // Find and modify document. If document doesnt exist, insert it
            FindAndModifyArgs findModifyArgs = new FindAndModifyArgs();
            findModifyArgs.SortBy = setSortBy;
            findModifyArgs.Query = cardQuery;
            findModifyArgs.Upsert = true;
            findModifyArgs.Update = setUpdate;
            findModifyArgs.VersionReturned = FindAndModifyDocumentVersion.Modified;

            var setResult = collection.FindAndModify(findModifyArgs);
            var setModified = setResult.GetModifiedDocumentAs<Set>();

            return setModified;
        }
        #endregion

        #region SetExists
        public async Task<bool> SetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.mSearchUtility.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

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

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

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

            name = this.mSearchUtility.GetRegexSearchValue(name);

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

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

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            // BsonRegEx in order to do a case insensitive match
            var query = Query<Set>.Matches(e => e.Code, new BsonRegularExpression(code, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var sets = collection.FindOne(query);

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return sets;
        }

        public async Task<List<Set>> GetSets()
        {
            List<Set> sets = new List<Set>();

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            MongoCursor<Set> cursor = collection.FindAll().SetSortOrder("name");

            foreach (Set set in cursor)
            {
                sets.Add(set);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return sets;
        }
        #endregion
    }
}
