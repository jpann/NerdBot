using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBotCardImage;
using NUnit.Framework;

namespace NerdBotCardImagePlugin_Tests
{
    [TestFixture]
    public class ImgCommand_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private ImgCommand imgCommandPlugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            mtgStore = new MtgStore(connectionString, databaseName);
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
            imgCommandPlugin = new ImgCommand(
                mtgStore,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);
        }

        #region Tests for cards that failed in live testing
        // This command was not returning any cards when it should have.
        [Test]
        public void ImgCommand_ByName_BreathStealer()
        {
            var cmd = new Command()
            {
                Cmd = "IMG",
                Arguments = new string[]
                {
                    "breath%stealer%"   
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage("https://api.mtgdb.info/content/hi_res_card_images/3278.jpg"));
        }

        [Test]
        public void ImgCommand_ByName_BreathSteelersCrypt()
        {
            var cmd = new Command()
            {
                Cmd = "IMG",
                Arguments = new string[]
                {
                    "breath%stealer%crypt"   
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage("https://api.mtgdb.info/content/hi_res_card_images/3734.jpg"));
        }
        #endregion

        [Test]
        public void ImgCommand_ByName()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByName_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSet()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSet_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSet_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSetCode()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSetCode_NameDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
        public void ImgCommand_ByNameAndSetCode_SetDoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "img",
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
