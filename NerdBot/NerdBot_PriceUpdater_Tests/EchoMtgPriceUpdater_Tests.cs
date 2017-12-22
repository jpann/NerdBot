using System;
using System.IO;
using System.Reflection;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBot_PriceUpdater.PriceUpdaters;
using NUnit.Framework;

namespace NerdBot_PriceUpdater_Tests
{
    [TestFixture]
    public class EchoMtgPriceUpdater_Tests
    {
        private TestConfiguration testConfig;

        private IPriceUpdater priceUpdater;
        private IMtgStore mtgStore;
        private UnitTestContext unitTestContext;
        private IHttpClient httpClient;

        #region Test Utility Methods
        private Set CreateSet()
        {
            var set = new Set()
            {
                Name = "Hour of Devastation",
                SearchName = "hourofdevastation",
                Block = "Amonkhet",
                Type = "expansion",
                Code = "HOU",
                CommonQty = 74,
                UncommonQty = 67,
                RareQty = 49,
                MythicQty = 14,
                BasicLandQty = 15,
                ReleasedOn = new DateTime(2017, 7, 14),
                MagicCardsInfoCode = "hou"
            };

            return set;
        }

        private string ReadTestPage(string file)
        {
            string testSetFile = Path.Combine(Path.GetDirectoryName(AssemblyLocation()), file);

            string source = File.ReadAllText(testSetFile);

            return source;
        }

        private string AssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = codebase.LocalPath;
            return path;
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();
            httpClient = new SimpleHttpClient(unitTestContext.LoggingServiceMock.Object);

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.Database,
                unitTestContext.QueryStatisticsStoreMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);

            // Setup ICardPriceStore Mocks
            unitTestContext.PriceStoreMock.Setup(p => p.FindAndModifySetPrice(It.IsAny<SetPrice>(), true))
                .Returns((SetPrice price, bool upsert) => price);

            unitTestContext.PriceStoreMock.Setup(p => p.FindAndModifyCardPrice(It.IsAny<CardPrice>(), true))
                .Returns((CardPrice price, bool upsert) => price);

            priceUpdater = new EchoMtgPriceUpdater(
                mtgStore,
                unitTestContext.PriceStoreMock.Object,
                unitTestContext.HttpClientMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);
        }

        [Test]
        public void UpdatePrices_SingleSet()
        {
            string url = "https://www.echomtg.com/set/HOU/";

            Set set = CreateSet();

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource(url))
                .Returns(ReadTestPage("TestSet.html"));

            priceUpdater.UpdatePrices(set);

            unitTestContext.PriceStoreMock.Verify(p => 
                p.FindAndModifySetPrice(
                    It.Is<SetPrice>(setPrice => 
                        setPrice.Name == set.Name &&
                        setPrice.SetCode == set.Code), true), Times.Once);

            unitTestContext.PriceStoreMock.Verify(p =>
                p.FindAndModifyCardPrice(
                    It.Is<CardPrice>(card =>
                        card.SetCode == set.Code), true), Times.Exactly(199));
        }

        [Test]
        public void UpdatePrices()
        {
            string setsUrl = "https://www.echomtg.com/sets/";

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource(setsUrl))
                .Returns(ReadTestPage("TestAllSets.html"));

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource("https://www.echomtg.com/set/EO2/explorers-of-ixalan/"))
                .Returns(ReadTestPage("explorers-of-ixalan.html"));

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource("https://www.echomtg.com/set/V17/from-the-vault-transform/"))
                .Returns(ReadTestPage("from-the-vault-transform.html"));

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource("https://www.echomtg.com/set/UN3/unstable/"))
                .Returns(ReadTestPage("unstable.html"));

            priceUpdater.UpdatePrices();

            // Check if two set prices were saved. 2 because V17 doesnt exist on mtgjson so it doesn't exist in the database.
            unitTestContext.PriceStoreMock.Verify(p =>
                p.FindAndModifySetPrice(
                    It.IsAny<SetPrice>(), true), Times.Exactly(2));

            unitTestContext.PriceStoreMock.Verify(p =>
                p.FindAndModifyCardPrice(It.IsAny<CardPrice>(), true), Times.AtLeast(326));
        }
    }
}
