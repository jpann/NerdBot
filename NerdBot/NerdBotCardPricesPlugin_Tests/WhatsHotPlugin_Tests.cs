using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public class WhatsHotPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private WhatsHotPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
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

            // Setup IHttpClient Mocks
            httpClientMock = new Mock<IHttpClient>();

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
                .Returns(httpClientMock.Object);

            servicesMock.SetupGet(s => s.UrlShortener)
                .Returns(urlShortenerMock.Object);

            plugin = new WhatsHotPlugin(
                servicesMock.Object,
                new BotConfig());

            plugin.LoggingService = loggingServiceMock.Object;
        }

        [Test]
        public void WhatsHot()
        {
            // Setup ICardPriceStore mock
            priceStoreMock.Setup(ps => ps.GetCardsByPriceIncrease(It.IsAny<int>()))
                .Returns(() => new List<CardPrice>()
                {
                    new CardPrice()
                    {
                        Name = "Collected Company",
                        SearchName = "collectedcompany",
                        SetCode = "DTK",
                        PriceDiff = "106%",
                        PriceDiffValue = 106,
                        PriceMid = "$9.24",
                        PriceLow = "$7.61",
                        PriceFoil = "$31.50"
                    },
                    new CardPrice()
                    {
                        Name = "Dack Fayden",
                        SearchName = "dackfayden",
                        SetCode = "CNS",
                        PriceDiff = "72%",
                        PriceDiffValue = 72,
                        PriceMid = "$48.14",
                        PriceLow = "$34.99",
                        PriceFoil = "$329.82"
                    },
                    new CardPrice()
                    {
                        Name = "Inkmoth Nexus",
                        SearchName = "inkmothnexus",
                        SetCode = "MBS",
                        PriceDiff = "65%",
                        PriceDiffValue = 65,
                        PriceMid = "$23.00",
                        PriceLow = "$18.50",
                        PriceFoil = "$57.70",
                    },
                    new CardPrice()
                    {
                        Name = "Dimir Signet",
                        SearchName = "dimirsignet",
                        SetCode = "ARC",
                        PriceDiff = "46%",
                        PriceDiffValue = 46,
                        PriceMid = "$0.70",
                        PriceLow = "$0.38",
                        PriceFoil = "",
                    },
                    new CardPrice()
                    {
                        Name = "Elvish Archers",
                        SearchName = "elvisharchers",
                        SetCode = "LEB",
                        PriceDiff = "36%",
                        PriceDiffValue = 36,
                        PriceMid = "$90.00",
                        PriceLow = "$90.00",
                        PriceFoil = "",
                    },
                });

            var cmd = new Command()
            {
                Cmd = "whatshot"
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                    cmd,
                    msg,
                    messengerMock.Object
                    ).Result;

            messengerMock.Verify(m => 
                m.SendMessage("Today's Hot Cards - Collected Company [DTK]: $9.24 up 106%, Dack Fayden [CNS]: $48.14 up 72%, Inkmoth Nexus [MBS]: $23.00 up 65%, Dimir Signet [ARC]: $0.70 up 46%, Elvish Archers [LEB]: $90.00 up 36%"),
                Times.Once);
        }
    }
}
