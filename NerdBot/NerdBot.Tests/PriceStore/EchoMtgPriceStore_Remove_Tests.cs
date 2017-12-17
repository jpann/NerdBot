using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests.PriceStore
{
    [TestFixture]
    class EchoMtgPriceStore_Remove_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";
        private const string testDataTag = "http://TEST-DATA";

        private ICardPriceStore priceStore;
        private Mock<ILoggingService> loggingServiceMock;

        private CardPrice cardPriceToRemove;
        private Mock<SearchUtility> searchUtilityMock;
        
        #region Method to add test data
        private void AddTestData()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var card_collection = database.GetCollection<CardPrice>("echo_prices");
            var set_collection = database.GetCollection<SetPrice>("echo_set_prices");

            // Card 1
            CardPrice card1 = new CardPrice()
            {
                Name = "Remove me 1",
                SetCode = "XXX",
                PriceDiff = "1%",
                PriceLow = "$1.00",
                PriceMid = "$2.00",
                PriceFoil = "$3.00",
                SearchName = SearchHelper.GetSearchValue("Remove me 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-5)
            };

            CardPrice card2 = new CardPrice()
            {
                Name = "Remove me 2",
                SetCode = "XXX",
                PriceDiff = "12%",
                PriceLow = "$12.00",
                PriceMid = "$22.00",
                PriceFoil = "$32.00",
                SearchName = SearchHelper.GetSearchValue("Remove me 2"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-6)
            };

            CardPrice card3 = new CardPrice()
            {
                Name = "Remove me 3",
                SetCode = "XXX",
                PriceDiff = "2%",
                PriceLow = "$2.00",
                PriceMid = "$3.00",
                PriceFoil = "$4.00",
                SearchName = SearchHelper.GetSearchValue("Remove me 3"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-7)
            };

            cardPriceToRemove = new CardPrice()
            {
                Name = "Remove me X",
                SetCode = "XXX",
                PriceDiff = "1%",
                PriceLow = "$1.00",
                PriceMid = "$2.00",
                PriceFoil = "$3.00",
                SearchName = SearchHelper.GetSearchValue("Remove me X"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-2)
            };

            card_collection.Save(card1);
            card_collection.Save(card2);
            card_collection.Save(card3);
            card_collection.Save(cardPriceToRemove);

            SetPrice set1 = new SetPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                TotalCards = 1,
                SetValue = "$2.00",
                FoilSetValue = "$10.00",
                SearchName = SearchHelper.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            SetPrice set2 = new SetPrice()
            {
                Name = "Find Modify 2",
                SetCode = "YYY",
                TotalCards = 1,
                SetValue = "$2.00",
                FoilSetValue = "$10.00",
                SearchName = SearchHelper.GetSearchValue("Find Modify 2"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            SetPrice set3 = new SetPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                TotalCards = 1,
                SetValue = "$4.00",
                FoilSetValue = "$4.00",
                SearchName = SearchHelper.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            SetPrice setToRemove = new SetPrice()
            {
                Name = "Remove me 1",
                SetCode = "XXX",
                TotalCards = 1,
                SetValue = "$4.00",
                FoilSetValue = "$4.00",
                SearchName = SearchHelper.GetSearchValue("Remove me 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-2)
            };

            set_collection.Save(set1);
            set_collection.Save(set2);
            set_collection.Save(set3);
            set_collection.Save(setToRemove);
        }
        #endregion

        #region Method to remove test data
        private void RemoveTestData()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var card_collection = database.GetCollection<CardPrice>("echo_prices");
            var set_collection = database.GetCollection<SetPrice>("echo_set_prices");

            var cardquery = Query<CardPrice>.EQ(c => c.Url, testDataTag);
            var cardremoveResult = card_collection.Remove(cardquery);

            var setquery = Query<SetPrice>.EQ(c => c.Url, testDataTag);
            var setremoveResult = set_collection.Remove(setquery);
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetRegexSearchValue(s));

            priceStore = new EchoMtgPriceStore(
                connectionString, 
                databaseName, 
                loggingServiceMock.Object,
                searchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            this.AddTestData();
        }

        [TearDown]
        public void TearDown()
        {
            this.RemoveTestData();
        }

        #region RemoveSetPrice
        private SetPrice GetTestSet()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var collection = database.GetCollection<SetPrice>("echo_prices");

            var set = collection.FindOneAs<SetPrice>(
                Query<SetPrice>.EQ(c => c.Name, "Remove me 1"));

            return set;
        }

        [Test]
        public void RemoveSetPrice()
        {
            bool actual = priceStore.RemoveCardPrice(cardPriceToRemove);

            Assert.True(actual);
        }

        [Test]
        public void RemoveSetPrice_DoesntExist()
        {
            SetPrice set = new SetPrice()
            {
                Name = "Remove me 1",
                SetCode = "XXX",
                TotalCards = 1,
                SetValue = "$4.00",
                FoilSetValue = "$4.00",
                SearchName = SearchHelper.GetSearchValue("Remove me 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-5)
            };

            bool actual = priceStore.RemoveSetPrice(set);

            Assert.True(actual);
        }
        #endregion

        #region RemoveSetPricesOnOrBefore
        [Test]
        public void RemoveSetPriceOnOrBefore()
        {
            DateTime removeOnOrBefore = DateTime.Now.AddDays(-3);

            int actual = priceStore.RemoveSetPricesOnOrBefore(removeOnOrBefore);

            Assert.GreaterOrEqual(actual, 0);
        }

        [Test]
        public void RemoveSetPriceOnOrBefore_NothingRemoved()
        {
            DateTime removeOnOrBefore = DateTime.Now.AddDays(-30);

            int actual = priceStore.RemoveSetPricesOnOrBefore(removeOnOrBefore);

            Assert.GreaterOrEqual(actual, 0);
        }
        #endregion

        #region RemoveCardPrice
        private CardPrice GetTestCard()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var collection = database.GetCollection<CardPrice>("echo_prices");

            var card = collection.FindOneAs<CardPrice>(
                Query<CardPrice>.EQ(c => c.Name, "Remove me X"));

            return card;
        }

        [Test]
        public void RemoveCardPrice()
        {
            bool actual = priceStore.RemoveCardPrice(cardPriceToRemove);

            Assert.True(actual);
        }

        [Test]
        public void RemoveCardPrice_DoesntExist()
        {
            CardPrice card = new CardPrice()
            {
                Name = "Remove me",
                SetCode = "XXX",
                PriceDiff = "1%",
                PriceLow = "$1.00",
                PriceMid = "$2.00",
                PriceFoil = "$3.00",
                SearchName = SearchHelper.GetSearchValue("Remove me"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-5)
            };
            bool actual = priceStore.RemoveCardPrice(card);

            Assert.True(actual);
        }
        #endregion

        #region RemoveCardPricesOnOrBefore
        [Test]
        public void RemoveCardPricesOnOrBefore()
        {
            DateTime removeOnOrBefore = DateTime.Now.AddDays(-3);

            int actual = priceStore.RemoveCardPricesOnOrBefore(removeOnOrBefore);

            Assert.GreaterOrEqual(actual, 0);
        }

        [Test]
        public void RemoveCardPricesOnOrBefore_NothingRemoved()
        {
            DateTime removeOnOrBefore = DateTime.Now.AddDays(-30);

            int actual = priceStore.RemoveCardPricesOnOrBefore(removeOnOrBefore);

            Assert.GreaterOrEqual(actual, 0);
        }
        #endregion
    }
}
