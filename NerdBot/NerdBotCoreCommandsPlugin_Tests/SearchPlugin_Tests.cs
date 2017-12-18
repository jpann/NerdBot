using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotCommon.Utilities;
using NerdBotCoreCommands;
using NUnit.Framework;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    public class SearchPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private SearchPlugin plugin;

        private UnitTestContext unitTestContext;

        private Mock<SearchUtility> searchUtilityMock;

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

            plugin = new SearchPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void Search_ByName()
        {
            var cmd = new Command()
            {
                Cmd = "search",
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
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.Contains("/search/"))));
        }

        [Test]
        public void Search_NoNameProvided()
        {
            var cmd = new Command()
            {
                Cmd = "search",
                Arguments = new string[]
                {
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
