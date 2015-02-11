using System;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moq;
using NerdBot.Mtg.Prices;
using NerdBot.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests.PriceStore
{
    [TestFixture]
    class EchoMtgPriceStore_FindAndModify_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb_prices";
        private const string testDataTag = "http://TEST-DATA";

        private ICardPriceStore priceStore;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<SearchUtility> searchUtilityMock;

        public string GetSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        public string GetRegexSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Replace * and % with a regex '*' char
            searchValue = searchValue.Replace("%", ".*");

            // If the first character of the searchValue is not '*', 
            // meaning the user does not want to do a contains search,
            // explicitly use a starts with regex
            if (!searchValue.StartsWith(".*"))
            {
                searchValue = "^" + searchValue;
            }

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        #region Method to add test data
        private void AddTestData()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var collection = database.GetCollection<CardPrice>("echo_prices");

            // Card 1
            CardPrice card1 = new CardPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                PriceDiff = "1%",
                PriceLow = "$1.00",
                PriceMid = "$2.00",
                PriceFoil = "$3.00",
                SearchName = this.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            CardPrice card2 = new CardPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                PriceDiff = "12%",
                PriceLow = "$12.00",
                PriceMid = "$22.00",
                PriceFoil = "$32.00",
                SearchName = this.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            CardPrice card3 = new CardPrice()
            {
                Name = "Find Modify 2",
                SetCode = "XXX",
                PriceDiff = "2%",
                PriceLow = "$2.00",
                PriceMid = "$3.00",
                PriceFoil = "$4.00",
                SearchName = this.GetSearchValue("Find Modify 2"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-1)
            };

            collection.Save(card1);
            collection.Save(card2);
            collection.Save(card3);
        }
        #endregion

        #region Method to remove test data
        private void RemoveTestData()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var collection = database.GetCollection<CardPrice>("echo_prices");

            var query = Query<CardPrice>.EQ(c => c.Url, testDataTag);
            var removeResult = collection.Remove(query);
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

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

        #region FindAndModifyCardPrice
        [Test]
        public void FindAndModifyCardPrice()
        {
            CardPrice expected = new CardPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                PriceDiff = "10%",
                PriceLow = "$10.00",
                PriceMid = "$20.00",
                PriceFoil = "$30.00",
                SearchName = this.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now
            };

            CardPrice actual = priceStore.FindAndModifyCardPrice(expected, true);

            Assert.AreEqual(expected.PriceDiff, actual.PriceDiff);
            Assert.AreEqual(expected.PriceLow, actual.PriceLow);
            Assert.AreEqual(expected.PriceMid, actual.PriceMid);
            Assert.AreEqual(expected.PriceFoil, actual.PriceFoil);
        }

        [Test]
        public void FindAndModifyCardPrice_DoesntExist()
        {
            CardPrice expected = new CardPrice()
            {
                Name = "Find Modify 3",
                SetCode = "XXX",
                PriceDiff = "10%",
                PriceLow = "$10.00",
                PriceMid = "$20.00",
                PriceFoil = "$30.00",
                SearchName = this.GetSearchValue("Find Modify 3"),
                Url = testDataTag,
                LastUpdated = DateTime.Now
            };

            CardPrice actual = priceStore.FindAndModifyCardPrice(expected, true);

            Assert.AreEqual(expected.PriceDiff, actual.PriceDiff);
            Assert.AreEqual(expected.PriceLow, actual.PriceLow);
            Assert.AreEqual(expected.PriceMid, actual.PriceMid);
            Assert.AreEqual(expected.PriceFoil, actual.PriceFoil);
        }

        [Test]
        public void FindAndModifyCardPrice_MultipleButUpdateOne()
        {
            CardPrice expected = new CardPrice()
            {
                Name = "Find Modify 1",
                SetCode = "XXX",
                PriceDiff = "10%",
                PriceLow = "$10.00",
                PriceMid = "$20.00",
                PriceFoil = "$30.00",
                SearchName = this.GetSearchValue("Find Modify 1"),
                Url = testDataTag,
                LastUpdated = DateTime.Now
            };

            CardPrice actual = priceStore.FindAndModifyCardPrice(expected, true);

            Assert.AreEqual(expected.PriceDiff, actual.PriceDiff);
            Assert.AreEqual(expected.PriceLow, actual.PriceLow);
            Assert.AreEqual(expected.PriceMid, actual.PriceMid);
            Assert.AreEqual(expected.PriceFoil, actual.PriceFoil);
        }
        #endregion
    }
}
