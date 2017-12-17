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
using NerdBotCardImage;
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

namespace NerdBotCardImagePlugin_Tests
{
    [TestFixture]
    public class ImgHiresCommand_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private ImgHiresCommand imgCommandPlugin;

        private Mock<IBotServices> servicesMock;
        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<SearchUtility> searchUtilityMock;
        private Mock<IQueryStatisticsStore> queryStatisticsStoreMock;

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

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();
            imgCommandPlugin = new ImgHiresCommand(
                servicesMock.Object,
                new BotConfig());

            imgCommandPlugin.LoggingService = loggingServiceMock.Object;
        }

        [Test]
        public void ImgHiresCommand_ByName()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "Spore Cloud"   
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }

        [Test]
        public void ImgHiresCommand_ByName_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "Bore Cloud"   
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
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
        public void ImgHiresCommand_ByNameAndSet()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "Fallen Empires",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }

        [Test]
        public void ImgHiresCommand_ByNameAndSet_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "Fallen Empires",
                    "Bore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
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
        public void ImgHiresCommand_ByNameAndSet_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "Callen Empires",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
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
        public void ImgHiresCommand_ByNameAndSetCode()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "FEM",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }

        [Test]
        public void ImgHiresCommand_ByNameAndSetCode_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "FEM",
                    "Bore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
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
        public void ImgHiresCommand_ByNameAndSetCode_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "imghires",
                Arguments = new string[]
                {
                    "XXX",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
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
