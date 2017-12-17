using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.TestsHelper;
using NerdBotCardPrices;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    class EbayPricePlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private EbayPricePlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private IHttpClient httpClient;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<SearchUtility> searchUtilityMock;
        private Mock<IQueryStatisticsStore> queryStatisticsStoreMock;
        private Mock<IBotServices> servicesMock;
        
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();
            queryStatisticsStoreMock = new Mock<IQueryStatisticsStore>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetRegexSearchValue(s));

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.Database,
                queryStatisticsStoreMock.Object,
                loggingServiceMock.Object,
                searchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICardPriceStore Mocks
            priceStoreMock = new Mock<ICardPriceStore>();

            // Setup ICommandParser Mocks
            commandParserMock = new Mock<ICommandParser>();

            httpClient = new SimpleHttpClient(loggingServiceMock.Object);

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            // Setup IBotServices Mocks
            servicesMock = new Mock<IBotServices>();

            servicesMock.SetupGet(s => s.QueryStatisticsStore)
                .Returns(queryStatisticsStoreMock.Object);

            servicesMock.SetupGet(s => s.Store)
                .Returns(mtgStore);

            servicesMock.SetupGet(s => s.PriceStore)
                .Returns(priceStoreMock.Object);

            servicesMock.SetupGet(s => s.CommandParser)
                .Returns(commandParserMock.Object);

            servicesMock.SetupGet(s => s.HttpClient)
                .Returns(httpClient);

            servicesMock.SetupGet(s => s.UrlShortener)
                .Returns(urlShortenerMock.Object);

            plugin = new EbayPricePlugin(
                servicesMock.Object,
                new BotConfig());

            plugin.LoggingService = loggingServiceMock.Object;
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
                messengerMock.Object
            ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)))),
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
                messengerMock.Object
            ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)))),
                Times.Once);
        }

        [Test]
        public void GetPrice_ShortenedUrl()
        {
            string name = "Inquisition of Kozilek";
            string url = "http://www.ebay.com/xxxxxxx";

            urlShortenerMock.Setup(u => u.ShortenUrl(It.Is<string>(p => p.StartsWith("http://www.ebay.com/"))))
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
                messengerMock.Object
            ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => 
            s.StartsWith(string.Format("The eBay Buy It Now price for '{0}' is", name)) &&
            s.Contains(url))),
                Times.Once);
        }
    }
}
