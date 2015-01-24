using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBotCardImage;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCardImagePlugin_Tests
{
    [TestFixture]
    public class ImgHiresCommand_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private ImgHiresCommand imgCommandPlugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();

            mtgStore = new MtgStore(connectionString, databaseName, loggingServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
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
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);
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

            messengerMock.Verify(m => m.SendMessage("https://api.mtgdb.info/content/hi_res_card_images/1922.jpg"));
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

            messengerMock.Verify(m => m.SendMessage("https://api.mtgdb.info/content/hi_res_card_images/1922.jpg"));
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

            messengerMock.Verify(m => m.SendMessage("https://api.mtgdb.info/content/hi_res_card_images/1922.jpg"));
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
