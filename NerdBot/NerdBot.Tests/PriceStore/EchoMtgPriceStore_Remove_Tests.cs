using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    class EchoMtgPriceStore_Remove_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb_prices";
        private const string testDataTag = "http://TEST-DATA";

        private ICardPriceStore priceStore;
        private Mock<ILoggingService> loggingServiceMock;

        private CardPrice cardPriceToRemove;
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
                Name = "Remove me 1",
                SetCode = "XXX",
                PriceDiff = "1%",
                PriceLow = "$1.00",
                PriceMid = "$2.00",
                PriceFoil = "$3.00",
                SearchName = this.GetSearchValue("Remove me 1"),
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
                SearchName = this.GetSearchValue("Remove me 2"),
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
                SearchName = this.GetSearchValue("Remove me 3"),
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
                SearchName = this.GetSearchValue("Remove me X"),
                Url = testDataTag,
                LastUpdated = DateTime.Now.AddDays(-2)
            };

            collection.Save(card1);
            collection.Save(card2);
            collection.Save(card3);
            collection.Save(cardPriceToRemove);
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
                SearchName = this.GetSearchValue("Remove me"),
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
