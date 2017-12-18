using Moq;
using NerdBot.TestsHelper;
using NerdBotCardPrices;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NUnit.Framework;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    class EbayPricePlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private EbayPricePlugin plugin;
        private IHttpClient httpClient;
        private UnitTestContext unitTestContext;


        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.Database,
                unitTestContext.QueryStatisticsStoreMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);

            unitTestContext.BotServicesMock.SetupGet(b => b.Store)
                .Returns(mtgStore);

            httpClient = new SimpleHttpClient(unitTestContext.LoggingServiceMock.Object);

            unitTestContext.BotServicesMock.SetupGet(b => b.HttpClient)
                .Returns(httpClient);
            
            plugin = new EbayPricePlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void GetPrice_ByName()
        {
            string name = "Inquisition of Kozilek";

            var cmd = new Command()
            {
                Cmd = "ebay",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)))),
                Times.Once);
        }

        [Test]
        public void GetPrice_ByNameSet()
        {
            string name = "Inquisition of Kozilek";
            string set = "Rise of the Eldrazi";

            var cmd = new Command()
            {
                Cmd = "ebay",
                Arguments = new string[]
                {
                    set,
                    name
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)))),
                Times.Once);
        }

        [Test]
        public void GetPrice_ShortenedUrl()
        {
            string name = "Inquisition of Kozilek";
            string url = "http://www.ebay.com/xxxxxxx";

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl(It.Is<string>(p => p.StartsWith("http://www.ebay.com/"))))
                .Returns(() => url);

            var cmd = new Command()
            {
                Cmd = "ebay",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => 
            s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)) &&
            s.Contains(url))),
                Times.Once);
        }
    }
}
