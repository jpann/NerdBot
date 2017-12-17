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
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using NerdBotCoreCommands;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    public class RulingPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private RulingPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<IBotServices> servicesMock;
        private Mock<IQueryStatisticsStore> queryStatisticsStoreMock;
        private Mock<SearchUtility> searchUtilityMock;
        
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

            var config = new BotConfig();
            config.HostUrl = "http://localhost";

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

            plugin = new RulingPlugin(
                servicesMock.Object,
                config);

            plugin.LoggingService = loggingServiceMock.Object;
        }

        [Test]
        public void Ruling_ByName()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "Fallen Angel"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled = 
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.Contains("/ruling/"))));
        }

        [Test]
        public void Ruling_ByName_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "Bore Cloud"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }

        [Test]
        public void Ruling_ByNameAndSet()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "Duel Decks Anthology, Divine vs. Demonic",
                    "Fallen Angel"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.Contains("http://localhost/ruling/394029"))));
        }

        [Test]
        public void Ruling_ByNameAndSet_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "Fallen Empires",
                    "Bore Cloud"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }

        [Test]
        public void Ruling_ByNameAndSet_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "Ballen Empires",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }

        [Test]
        public void Ruling_ByNameAndSetCode()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "DD3_DVD",
                    "Fallen Angel"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.Contains("http://localhost/ruling/394029"))));
        }

        [Test]
        public void Ruling_ByNameAndSetCode_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "FEM",
                    "Bore Cloud"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }

        [Test]
        public void Ruling_ByNameAndSetCode_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "ruling",
                Arguments = new string[]
                {
                    "XXX",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }
    }
}
