using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.TestsHelper;
using NerdBot.UrlShortners;
using NerdBot.Utilities;
using NerdBotCardImage;
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

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<SearchUtility> searchUtilityMock;

        public string GetSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        public string GetRegexSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Replace * and % with a regex '*' char
            searchValue = searchValue.Replace("%", ".*");

            // If the first character of the searchValue is not '*', 
            // meaning the user does not want to do a contains search,
            // explicitly use a starts with regex
            if (!searchValue.StartsWith(".*"))
            {
                searchValue = "^" + searchValue;
            }

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            mtgStore = new MtgStore(
                testConfig.Url, 
                testConfig.Database, 
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
            imgCommandPlugin = new ImgHiresCommand(
                mtgStore,
                priceStoreMock.Object,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object,
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

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("1922.jpg"))));
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

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("1922.jpg"))));
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

            messengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("1922.jpg"))));
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
