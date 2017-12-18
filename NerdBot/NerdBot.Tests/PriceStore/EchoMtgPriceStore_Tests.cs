using System.Collections.Generic;
using NerdBot.TestsHelper;
using NerdBotCommon.Mtg.Prices;
using NUnit.Framework;

namespace NerdBot.Tests.PriceStore
{
    [TestFixture]
    class EchoMtgPriceStore_Tests
    {
        private TestConfiguration testConfig;

        private ICardPriceStore priceStore;
        private UnitTestContext unitTestContext = new UnitTestContext();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            priceStore = new EchoMtgPriceStore(
                testConfig.Url, 
                testConfig.PriceDatabase,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        //TODO SetPrice tests

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
