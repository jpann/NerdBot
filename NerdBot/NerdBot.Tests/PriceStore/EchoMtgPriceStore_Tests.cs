using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq;
using NerdBot.Mtg.Prices;
using NerdBot.TestsHelper;
using NerdBot.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests.PriceStore
{
    [TestFixture]
    class EchoMtgPriceStore_Tests
    {
        private TestConfiguration testConfig;

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

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            priceStore = new EchoMtgPriceStore(
                testConfig.Url, 
                testConfig.PriceDatabase, 
                loggingServiceMock.Object, 
                searchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        #region GetCardPrice
        [Test]
        public void GetCardPrice_ByName()
        {
            string name = "Dr%";

            CardPrice actual = priceStore.GetCardPrice(name);

            Assert.NotNull(actual);
        }

        [Test]
        public void GetCardPrice_ByName_DoesntExist()
        {
            string name = "Goblin War XXXX";

            CardPrice actual = priceStore.GetCardPrice(name);

            Assert.Null(actual);
        }

        [Test]
        public void GetCardPrice_ByNameSet()
        {
            string name = "%a%";
            string setCode = "DTK";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.NotNull(actual);
        }

        [Test]
        public void GetCardPrice_ByNameSet_NameDoesntExist()
        {
            string name = "Goblin War XXXX";
            string setCode = "ZEN";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.Null(actual);
        }

        [Test]
        public void GetCardPrice_ByNameSet_SetDoesntExist()
        {
            string name = "Goblin War Paint";
            string setCode = "XXX";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.Null(actual);
        }
        #endregion

        #region InsertCardPrice
        #endregion

        #region RemoveCardPrice
        #endregion

        #region FindAndModifyCardPrice

        #endregion

        #region GetCardsByPriceIncrease
        [Test]
        public void GetCardsByPriceIncrease()
        {
            int limit = 10;

            List<CardPrice> prices = priceStore.GetCardsByPriceIncrease(limit);

            Assert.AreEqual(limit, prices.Count);
        }
        #endregion

        #region GetCardsByPriceDecrease
        [Test]
        public void GetCardsByPriceDecrease()
        {
            int limit = 10;

            List<CardPrice> prices = priceStore.GetCardsByPriceDecrease(limit);

            Assert.AreEqual(limit, prices.Count);
        }
        #endregion
    }
}
