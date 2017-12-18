using System;
using NerdBot.TestsHelper;
using NerdBotCardPrices.PriceFetchers;
using NerdBotCommon.Http;
using NUnit.Framework;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    class EbayPriceFetcher_Tests
    {
        private EbayPriceFetcher priceFetcher;

        private IHttpClient httpClient;
        private UnitTestContext unitTestContext;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            httpClient = new SimpleHttpClient(unitTestContext.LoggingServiceMock.Object);

            priceFetcher = new EbayPriceFetcher(httpClient, unitTestContext.LoggingServiceMock.Object);
        }

        [Test]
        public void GetPrice_ByName()
        {
            string name = "Spore Cloud";

            string[] actual = priceFetcher.GetPrice(name);

            Assert.AreEqual(2, actual.Length);
            Assert.True(actual[0].StartsWith("$"));
            Assert.True(actual[1].Contains(Uri.EscapeDataString(name)));
        }

        [Test]
        public void GetPrice_ByNameSet()
        {
            string name = "Spore Cloud";
            string set = "Fallen Empires";

            string[] actual = priceFetcher.GetPrice(name, set);

            Assert.AreEqual(2, actual.Length);
            Assert.True(actual[0].StartsWith("$"));
            Assert.True(actual[1].Contains(Uri.EscapeDataString(name)));
            Assert.True(actual[1].Contains(Uri.EscapeDataString(set)));
        }
    }
}
