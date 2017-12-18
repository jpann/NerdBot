﻿using Moq;
using NerdBot.TestsHelper;
using NerdBotCardImage;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NUnit.Framework;

namespace NerdBotCardImagePlugin_Tests
{
    [TestFixture]
    public class ImgCommand_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private ImgCommand imgCommandPlugin;

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

            imgCommandPlugin = new ImgCommand(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            imgCommandPlugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("3278.jpg"))));
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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("3734.jpg"))));
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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }

        [Test]
        public void ImgCommand_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "img",
                Arguments = new string[]
                {
                    ""
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }

        [Test]
        public void ImgCommand_ByNameAndSet_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "img",
                Arguments = new string[]
                {
                    "Fallen Empires",
                    ""
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
            Assert.False(handled);
        }

        [Test]
        public void ImgCommand_ByNameAndSet_NoSet()
        {
            var cmd = new Command()
            {
                Cmd = "img",
                Arguments = new string[]
                {
                    "",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();


            bool handled = imgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
            Assert.False(handled);
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
                unitTestContext.MessengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);

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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith(".jpg"))));
        }
    }
}
