using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotCoreCommands;
using NUnit.Framework;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    public class GetSetPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private GetSetPlugin plugin;

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

            plugin = new GetSetPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void GetSet_ByName()
        {
            var cmd = new Command()
            {
                Cmd = "set",
                Arguments = new string[]
                {
                    "fallen empires"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Set 'Fallen Empires' [FEM] was released on 11-15-1994 and contains 187 cards."));
        }

        [Test]
        public void GetSet_ByName_DoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "set",
                Arguments = new string[]
                {
                    "ballen empires"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }

        [Test]
        public void GetSet_ByCode()
        {
            var cmd = new Command()
            {
                Cmd = "set",
                Arguments = new string[]
                {
                    "fem"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Set 'Fallen Empires' [FEM] was released on 11-15-1994 and contains 187 cards."));
        }

        [Test]
        public void GetSet_ByCode_DoesntExist()
        {
            var cmd = new Command()
            {
                Cmd = "set",
                Arguments = new string[]
                {
                    "XXX"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            // Verify that the messenger's SendMessenger was never called 
            //  because no message should be sent when a card was not found
            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);

            Assert.False(handled);
        }
        
    }
}
