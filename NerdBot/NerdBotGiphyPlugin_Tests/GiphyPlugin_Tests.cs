using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotGiphyPlugin;
using NUnit.Framework;

namespace NerdBotGiphyPlugin_Tests
{
    [TestFixture]
    class GiphyPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private GiphyPlugin plugin;
        private IHttpClient httpClient;

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

            httpClient = new SimpleHttpClient(unitTestContext.LoggingServiceMock.Object);

            unitTestContext.BotServicesMock.SetupGet(b => b.HttpClient)
                .Returns(httpClient);

            plugin = new GiphyPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void Giphy_Call()
        {
            var cmd = new Command()
            {
                Cmd = "giphy",
                Arguments = new string[]
                {
                    "geek"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("giphy.gif"))), Times.AtLeastOnce);
        }
    }
}
