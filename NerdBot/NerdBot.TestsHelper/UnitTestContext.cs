using System.Collections.Generic;
using Moq;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.Reporters;
using NerdBotCommon.Factories;
using NerdBotCommon.Http;
using NerdBotCommon.Importer;
using NerdBotCommon.Importer.DataReaders;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using SimpleLogging.Core;

namespace NerdBot.TestsHelper
{
    public class UnitTestContext
    {
        public string BotName { get; set; }
        public string BotId { get; set; }
        public string[] SecretToken { get; set; }
        public string[] SetcretTokenBad { get; set; }
        public BotConfig BotConfig { get; set; }

        public Mock<ILoggingService> LoggingServiceMock { get; set; }
        public Mock<SearchUtility> SearchUtilityMock { get; set; }
        public Mock<IMtgStore> StoreMock { get; set; }
        public Mock<IHttpClient> HttpClientMock { get; set; }
        public Mock<ICardPriceStore> PriceStoreMock { get; set; }
        public Mock<IMtgDataReader> MtgDataReaderMock { get; set; }
        public Mock<IImporter> ImporterMock { get; set; }
        public Mock<IPluginManager> PluginManagerMock { get; set; }
        public Mock<IMessenger> MessengerMock { get; set; }
        public Mock<IMessengerFactory> MessengerFactoryMock { get; set; }
        public Mock<ICommandParser> CommandParserMock { get; set; }
        public Mock<IReporter> ReporterMock { get; set; }
        public Mock<IBotServices> BotServicesMock { get; set; }
        public Mock<IQueryStatisticsStore> QueryStatisticsStoreMock { get; set; }
        public Mock<IUrlShortener> UrlShortenerMock { get; set; }

        public UnitTestContext()
        {
            BotName = "BotName";
            BotId = "BOTID";

            SecretToken = new string[] { "TOKEN" };
            SetcretTokenBad = new string[] { "BADTOKEN" };

            // Setup BotConfig
            BotConfig = new BotConfig()
            {
                HostUrl = "http://localhost",
                BotName = BotName,
                BotRoutes = new List<BotRoute>()
                {
                    new BotRoute()
                    {
                        SecretToken = SecretToken[0],
                        BotId = BotId
                    }
                }
            };

            // Setup ILoggingService Mock
            LoggingServiceMock = new Mock<ILoggingService>();

            // Setup SearchUtility Mock
            SearchUtilityMock = new Mock<SearchUtility>();

            SearchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetSearchValue(s));

            SearchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetRegexSearchValue(s));

            // Setup IMtgStore Mock
            StoreMock = new Mock<IMtgStore>();

            // Setup IHttpClientMock
            HttpClientMock = new Mock<IHttpClient>();

            // Setup ICardPriceStore Mock
            PriceStoreMock = new Mock<ICardPriceStore>();

            // Setup IMtgDataReader Mock
            MtgDataReaderMock = new Mock<IMtgDataReader>();

            // Setup IImporter Mock
            ImporterMock = new Mock<IImporter>();

            // Setup IPluginManager
            PluginManagerMock = new Mock<IPluginManager>();

            // Setup IMessenger Mock
            MessengerMock = new Mock<IMessenger>();

            MessengerMock.Setup(p => p.BotName)
                .Returns(BotName);

            MessengerMock.Setup(p => p.BotId)
                .Returns(BotId);

            // Setup IMessengerFactory Mock
            MessengerFactoryMock = new Mock<IMessengerFactory>();

            MessengerFactoryMock.Setup(c => c.Create(SecretToken[0]))
                .Returns(() => MessengerMock.Object);

            // Setup ICommandParser Mock
            CommandParserMock = new Mock<ICommandParser>();

            // Setup IReporter Mock
            ReporterMock = new Mock<IReporter>();

            // Setup IQueryStatisticsStore Mock
            QueryStatisticsStoreMock = new Mock<IQueryStatisticsStore>();

            // Setup IUrlShortener Mock
            UrlShortenerMock = new Mock<IUrlShortener>();

            // Setup IBotServices Mock
            BotServicesMock = new Mock<IBotServices>();

            BotServicesMock.SetupGet(s => s.QueryStatisticsStore)
                .Returns(QueryStatisticsStoreMock.Object);

            BotServicesMock.SetupGet(s => s.Store)
                .Returns(StoreMock.Object);

            BotServicesMock.SetupGet(s => s.PriceStore)
                .Returns(PriceStoreMock.Object);

            BotServicesMock.SetupGet(s => s.CommandParser)
                .Returns(CommandParserMock.Object);

            BotServicesMock.SetupGet(s => s.HttpClient)
                .Returns(HttpClientMock.Object);

            BotServicesMock.SetupGet(s => s.UrlShortener)
                .Returns(UrlShortenerMock.Object);
        }
    }
}