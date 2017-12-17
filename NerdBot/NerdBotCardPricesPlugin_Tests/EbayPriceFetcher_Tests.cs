using System;
using Moq;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBotCardPrices.PriceFetchers;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    class EbayPriceFetcher_Tests
    {
        private EbayPriceFetcher priceFetcher;
        private IHttpClient httpClient;

        private Mock<ILoggingService> loggingServiceMock;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();

            
        }

        [SetUp]
        public void SetUp()
        {
            httpClient = new SimpleHttpClient(loggingServiceMock.Object);

            loggingServiceMock = new Mock<ILoggingService>();
            
            priceFetcher = new EbayPriceFetcher(httpClient, loggingServiceMock.Object);
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
