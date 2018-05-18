using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotScryFallPlugin;
using NUnit.Framework;

namespace NerdBotScryFallPlugin_Tests
{
    public class ScryFallFetcher_Tests
    {
        private Mock<IHttpClient> httpClientMock;
        private TestData testData;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testData = new TestData();
        }

        [Test]
        public void GetCard_ByMultiverseId()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard.MultiverseId).Result;

            Assert.NotNull(actual);
        }

        [Test]
        public void GetCard_ByMultiverseId_NotFound()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard_ScryNotFound.MultiverseId).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByName_Fuzzy()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&fuzzy=" + testData.TestCard.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard.Name, true).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByName_Fuzzy_NotFound()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&fuzzy=" + testData.TestCard_ScryNotFound.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard_ScryNotFound.MultiverseId).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByName_NotFuzzy()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&exact=" + testData.TestCard.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard.Name, true).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByName_NotFuzzy_NotFound()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&exact=" + testData.TestCard_ScryNotFound.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard_ScryNotFound.MultiverseId).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByNameSet_NotFuzzy()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&exact=" + testData.TestCard.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard.Name, true).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByNameSet_NotFuzzy_NotFound()
        {
            // Setup IHttpClientMock
            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/named?&exact=" + testData.TestCard_ScryNotFound.Name))
                .ReturnsAsync("");

            var fetcher = new ScryFallFetcher(httpClientMock.Object);

            var actual = fetcher.GetCard(testData.TestCard_ScryNotFound.MultiverseId).Result;

            Assert.Null(actual);
        }
    }
}
