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
    class GetRandomCardByDescriptionPlugin_Tests
    {
        private TestConfiguration testConfig;
        private IMtgStore mtgStore;
        private GetRandomCardByDescriptionPlugin plugin;

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

            plugin = new GetRandomCardByDescriptionPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void GetRandomCardByDescription()
        {
            var cmd = new Command()
            {
                Cmd = "desc",
                Arguments = new string[]
                {
                    "shroud"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(c => c.EndsWith(".jpg"))), Times.Once);
        }

        [Test]
        public void GetRandomCardByDescription_MultipleSets()
        {
            var cmd = new Command()
            {
                Cmd = "desc",
                Arguments = new string[]
                {
                    "a"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(c => c.EndsWith(".jpg"))), Times.Once);
            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(c => c.Contains("Also in sets:"))), Times.Once);
        }
    }
}
