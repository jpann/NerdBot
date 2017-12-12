using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NerdBotCommon.Utilities;
using SimpleLogging.Core;

namespace NerdBotCommon.Mtg
{
    public class MtgStore : IMtgStore
    {
        private const string cCardsCollectionName = "cards";
        private const string cSetsCollectionName = "sets";

        private readonly string mConnectionString;
        private readonly string mDatabaseName;
        private readonly IMongoClient mClient;
        private readonly IMongoDatabase mDatabase;
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
            this.mDatabase = this.mClient.GetDatabase(this.mDatabaseName);
            this.mLoggingService = loggingService;
            this.mSearchUtility = searchUtility;
        }

        #region AddCard
        public async Task<Card> AddCard(Card card)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            collection.InsertOne(card);

            return card;
        }

        public async Task<Card> CardFindAndModify(Card card)
        {
            if (card == null)
                throw new ArgumentNullException("card");

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            // Query for this card
            var cardQuery = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Eq("name", card.Name),
                Builders<Card>.Filter.Eq("setId", card.SetId),
                Builders<Card>.Filter.Eq("multiverseId", card.MultiverseId)
                );

            var cardSoryBy = Builders<Card>.Sort.Descending("name");

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
            var cardUpdate = Builders<Card>.Update
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

            var options = new FindOneAndUpdateOptions<Card>();
            options.Sort = cardSoryBy;
            options.IsUpsert = true;
            options.ReturnDocument = ReturnDocument.After;

            var cardResult = collection.FindOneAndUpdate(cardQuery, cardUpdate, options);

            return cardResult;
        }
        #endregion

        #region CardExists
        public async Task<bool> CardExists(int multiverseId)
        {
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Builders<Card>.Filter.Eq(e => e.MultiverseId, multiverseId);

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
            
            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Eq(e => e.SearchName, name),
                Builders<Card>.Filter.Ne("multiverseId", 0));

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

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Eq(e => e.SearchName, name),
                Builders<Card>.Filter.Eq(e => e.SetSearchName, setName),
                Builders<Card>.Filter.Ne("multiverseId", 0)
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

            var query = Builders<Card>.Filter.Eq(e => e.MultiverseId, multiverseId);
            var sortBy = Builders<Card>.Sort.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.Find<Card>(query).Sort(sortBy).Limit(1);

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

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Builders<Card>.Filter.Ne("multiverseId", 0));

            var sortBy = Builders<Card>.Sort.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.Find<Card>(query).Sort(sortBy).Limit(1);

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
            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Builders<Card>.Filter.Ne("multiverseId", 0),
                Builders<Card>.Filter.Or(
                    Builders<Card>.Filter.Regex(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                    Builders<Card>.Filter.Regex(e => e.SetId, new BsonRegularExpression(setCode, "i")))
                );

            var sortBy = Builders<Card>.Sort.Ascending("searchName");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cardResult = collection.Find<Card>(query).Sort(sortBy).Limit(1);

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

            var cursor = collection.Find(new BsonDocument())
                .Sort(Builders<Card>.Sort.Ascending("searchName")).ToCursor();

            foreach (Card card in cursor.ToEnumerable())
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
				                
            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(name, "i")),
                Builders<Card>.Filter.Ne("multiverseId", 0));

            var filter = collection.Find(query)
                .Sort(Builders<Card>.Sort.Ascending("searchName"));

            IAsyncCursor<Card> cursor = null;

            if (limit > 0)
                cursor = filter.Limit(limit).ToCursor();
            else
                cursor = filter.ToCursor();

            foreach (Card card in cursor.ToEnumerable())
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }

		//public async Task<List<Card>> SearchCards(string name, int limit = 0)
		//{
		//	if (string.IsNullOrEmpty(name))
		//		throw new ArgumentException("name");

		//	List<Card> cards = new List<Card>();

		//	string regex_name = this.mSearchUtility.GetRegexSearchValue(name);

		//	var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

		//	Stopwatch watch = new Stopwatch();
		//	watch.Start();

		//	IMongoQuery query = null;
		//	MongoCursor<Card> cursor = null;

		//	string[] names = name.Split(' ');
		//	if (names.Length > 1)
		//	{
		//		List<IMongoQuery> nameQueries = new List<IMongoQuery>();

		//		foreach (string n in names)
		//		{
		//			nameQueries.Add(Query.And(
  //                      Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(n, "i")),
  //                      Query.NE("multiverseId", 0)));
		//		}

		//		cursor = collection.Find(Query.Or(nameQueries))
		//			.SetSortOrder("searchName");
		//	}
		//	else
		//	{
		//		query = Query.And(
  //                  Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(regex_name, "i")),
  //                  Query.NE("multiverseId", 0));

		//		cursor = collection.Find(query)
		//			.SetSortOrder("searchName");
		//	}

		//	if (limit > 0)
		//		cursor.SetLimit(limit);

		//	foreach (Card card in cursor)
		//	{
		//		cards.Add(card);
		//	}

		//	watch.Stop();

		//	this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

		//	return cards;
		//}

        //public async Task<List<Card>> AdvancedSearchCards(string name, int limit = 0)
        //{
        //    if (string.IsNullOrEmpty(name))
        //        throw new ArgumentException("name");

        //    List<Card> cards = new List<Card>();

        //    string regex_name = this.mSearchUtility.GetRegexAdvancedSearchValue(name);

        //    var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();

        //    IMongoQuery query = null;
        //    MongoCursor<Card> cursor = null;

        //    string[] names = name.Split(' ');
        //    if (names.Length > 1)
        //    {
        //        List<IMongoQuery> nameQueries = new List<IMongoQuery>();

        //        foreach (string n in names)
        //        {
        //            nameQueries.Add(Query.And(
        //                Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(this.mSearchUtility.GetRegexAdvancedSearchValue(n), "i")),
        //                Query.NE("multiverseId", 0)));
        //        }

        //        cursor = collection.Find(Query.And(nameQueries))
        //            .SetSortOrder("searchName");
        //    }
        //    else
        //    {
        //        query = Query.And(
        //            Query<Card>.Matches(e => e.SearchName, new BsonRegularExpression(regex_name, "i")),
        //            Query.NE("multiverseId", 0));

        //        cursor = collection.Find(query)
        //            .SetSortOrder("searchName");
        //    }

        //    if (limit > 0)
        //        cursor.SetLimit(limit);

        //    foreach (Card card in cursor)
        //    {
        //        cards.Add(card);
        //    }

        //    watch.Stop();

        //    this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

        //    return cards;
        //}

        public async Task<List<Card>> SearchCards(string name, int skipRecords = 0, int limit = 100)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            List<Card> cards = new List<Card>();

            string regex_name = this.mSearchUtility.GetRegexSearchValue(name);

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            FilterDefinition<Card> query = null;
            IAsyncCursor<Card> cursor = null;

            string[] names = name.Split(' ');
            if (names.Length > 1)
            {
                List<FilterDefinition<Card>> nameQueries = new List<FilterDefinition<Card>>();

                foreach (string n in names)
                {
                    nameQueries.Add(Builders<Card>.Filter.And(
                        Builders<Card>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(n, "i")),
                        Builders<Card>.Filter.Ne("multiverseId", 0)));
                }

                cursor = collection.Find(Builders<Card>.Filter.Or(nameQueries))
                    .Skip(skipRecords)
                    .Limit(limit)
                    .Sort(Builders<Card>.Sort.Ascending("searchName")).ToCursor();
            }
            else
            {
                query = Builders<Card>.Filter.And(
                    Builders<Card>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(regex_name, "i")),
                    Builders<Card>.Filter.Ne("multiverseId", 0));

                cursor = collection.Find(query)
                    .Skip(skipRecords)
                    .Limit(limit)
                    .Sort(Builders<Card>.Sort.Ascending("searchName")).ToCursor();
            }

            foreach (Card card in cursor.ToEnumerable())
            {
                cards.Add(card);
            }

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return cards;
        }

        public async Task<List<Card>> FullTextSearch(string term, int limit = 1000)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentException("term");

            List<Card> cards = new List<Card>();

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var filter = Builders<Card>.Filter.Text(term);
            var projection = Builders<Card>.Projection.MetaTextScore("TextMatchScore");
            var sort = Builders<Card>.Sort.MetaTextScore("TextMatchScore");

            cards = collection.Find(filter)
                .Project<Card>(projection)
                .Sort(sort)
                .Limit(limit)
                .ToList();

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

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Regex(e => e.Artist, new BsonRegularExpression(artist, "i")),
                Builders<Card>.Filter.Ne("multiverseId", 0));

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

            var query = Builders<Card>.Filter.And(
                    Builders<Card>.Filter.Ne("multiverseId", 0),
                    Builders<Card>.Filter.Or(
                        Builders<Card>.Filter.Regex(e => e.SetSearchName, new BsonRegularExpression(setName, "i")),
                        Builders<Card>.Filter.Regex(e => e.SetId, new BsonRegularExpression(setCode, "i")))
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

            int count = (int) collection.Count(new BsonDocument());
            var random = new Random();
            var r = random.Next(count);
            var card = collection.Find(new BsonDocument()).Skip(r).FirstOrDefault();

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

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Ne("multiverseId", 0),
                Builders<Card>.Filter.Regex(e => e.Desc, new BsonRegularExpression(text, "i")));

            var sortBy = Builders<Card>.Sort.Ascending("multiverseId");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Find(query).Count();

            var rand = new Random();
            var r = rand.Next(count);
            var card = collection.Find(query).Sort(sortBy).Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }

        public async Task<Card> GetRandomCardWithDescription(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("text");

            text = text.Replace("%", ".*");
            text = text.Replace("*", ".*");

            if (!text.EndsWith(".*"))
                text = text + ".*";

            text = ".*" + text;

            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Regex(e => e.Desc, new BsonRegularExpression(text, "i")),
                Builders<Card>.Filter.Ne("multiverseId", 0));

            var sortBy = Builders<Card>.Sort.Ascending("multiverseId");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int count = (int)collection.Find(query).Count();

            var rand = new Random();
            var r = rand.Next(count);
            var card = collection.Find(query).Sort(sortBy).Skip(r).FirstOrDefault();

            watch.Stop();

            this.mLoggingService.Trace("Elapsed time: {0}", watch.Elapsed);

            return card;
        }

        public async Task<string> GetRandomFlavorText()
        {
            var collection = this.mDatabase.GetCollection<Card>(cCardsCollectionName);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Ne("flavor", BsonString.Empty),
                Builders<Card>.Filter.Ne("flavor", BsonNull.Value),
                Builders<Card>.Filter.Ne("multiverseId", 0));

            int count = (int)collection.Find(query).Count();

            var random = new Random();
            var r = random.Next(count);
            Card card = collection.Find(new BsonDocument()).Skip(r).FirstOrDefault();

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
            var cardMultiverseIdQuery = Builders<Card>.Filter.Eq(e => e.MultiverseId, multiverseId);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.Find(cardMultiverseIdQuery).FirstOrDefault();

            if (card != null)
            {
                // Queries to get all other cards that share the card's name
                var cardNameQuery = Builders<Card>.Filter.And(
                    Builders<Card>.Filter.Ne("multiverseId", 0),
                    Builders<Card>.Filter.Eq(e => e.SearchName, card.SearchName));

                IAsyncCursor<Card> cursor = cardsCollection.Find(cardNameQuery)
                    .Sort(Builders<Card>.Sort.Ascending("setName")).ToCursor();

                List<FilterDefinition<Set>> setQueries = new List<FilterDefinition<Set>>();

                // Go through each card that shares this name and add create a set query for it
                foreach (Card c in cursor.ToEnumerable())
                {
                    setQueries.Add(Builders<Set>.Filter.Eq(e => e.SearchName, c.SetSearchName));
                }

                // Get all sets for cards that share this name
                IAsyncCursor<Set> setCursor = setsCollection.Find(Builders<Set>.Filter.Or(setQueries))
                    .Sort(Builders<Set>.Sort.Ascending("name"))
                    .Limit(limit).ToCursor();

                foreach (Set s in setCursor.ToEnumerable())
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
            var cardMultiverseIdQuery = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Ne("multiverseId", 0),
                Builders<Card>.Filter.Eq(e => e.MultiverseId, multiverseId));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var card = cardsCollection.Find(cardMultiverseIdQuery).FirstOrDefault();

            if (card != null)
            {
                // Queries to get all other cards that do not have this multiverseId but share the card's name
                var cardNotMultiverseIdQuery = Builders<Card>.Filter.And(
                    Builders<Card>.Filter.Ne("multiverseId", 0),
                    Builders<Card>.Filter.Ne(e => e.MultiverseId, multiverseId));

                var cardNameQuery = Builders<Card>.Filter.Eq(e => e.SearchName, card.SearchName);

                var cardOtherQuery = Builders<Card>.Filter.And(
                    cardNotMultiverseIdQuery,
                    cardNameQuery
                    );

                IAsyncCursor<Card> cursor = cardsCollection.Find(cardOtherQuery)
                    .Sort(Builders<Card>.Sort.Ascending("name")).ToCursor();

                List<FilterDefinition<Set>> setQueries = new List<FilterDefinition<Set>>();

                // Go throug each card that shares this name and add create a set query for it
                foreach (Card c in cursor.ToEnumerable())
                {
                    setQueries.Add(Builders<Set>.Filter.Eq(e => e.SearchName, c.SetSearchName));
                }

                if (setQueries.Any())
                {
                    // Get all sets for cards that share this name
                    IAsyncCursor<Set> setCursor = setsCollection.Find(Builders<Set>.Filter.Or(setQueries))
                        .Sort(Builders<Set>.Sort.Ascending("name")).ToCursor();

                    foreach (Set s in setCursor.ToEnumerable())
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

            var query = Builders<Card>.Filter.And(
                Builders<Card>.Filter.Ne("multiverseId", 0),
                Builders<Card>.Filter.Eq(e => e.SetSearchName, setName));

            Stopwatch watch = new Stopwatch();
            watch.Start();

           var cursor = collection.Find(query)
                .Sort(Builders<Card>.Sort.Ascending("name")).ToCursor();

            foreach (Card card in cursor.ToEnumerable())
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

            collection.InsertOne(set);

            return set;
        }

        public async Task<Set> SetFindAndModify(Set set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            // Query for this set
            var setQuery = Builders<Set>.Filter.And(
                Builders<Set>.Filter.Eq("name", set.Name),
                Builders<Set>.Filter.Eq("code", set.Code)
                );

            var setSortBy = Builders<Set>.Sort.Descending("name");

            // Update document
            var setUpdate = Builders<Set>.Update
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
            var options = new FindOneAndUpdateOptions<Set>();
            options.Sort = setSortBy;
            options.IsUpsert = true;
            options.ReturnDocument = ReturnDocument.After;

            var setResult = collection.FindOneAndUpdate(setQuery, setUpdate, options);

            return setResult;
        }
        #endregion

        #region SetExists
        public async Task<bool> SetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = this.mSearchUtility.GetSearchValue(name);

            var collection = this.mDatabase.GetCollection<Set>(cSetsCollectionName);

            var query = Builders<Set>.Filter.Eq(e => e.SearchName, name);
     
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

            var query = Builders<Set>.Filter.Eq(e => e.Code, code);

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

            var query = Builders<Set>.Filter.Regex(e => e.SearchName, new BsonRegularExpression(name, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var set = collection.Find(query).FirstOrDefault();

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
            var query = Builders<Set>.Filter.Regex(e => e.Code, new BsonRegularExpression(code, "i"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var sets = collection.Find(query).FirstOrDefault();

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

            var cursor = collection.Find(new BsonDocument()).Sort(Builders<Set>.Sort.Ascending("name")).ToCursor();

            foreach (Set set in cursor.ToEnumerable())
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
