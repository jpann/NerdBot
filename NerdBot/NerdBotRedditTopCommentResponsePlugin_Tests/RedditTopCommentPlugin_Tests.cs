using Moq;
using NerdBotRedditTopCommentResponsePlugin;
using NUnit.Framework;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
namespace NerdBotRedditTopCommentResponsePlugin_Tests
{
    [TestFixture]
    public class RedditTopCommentPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private RedditTopCommentPlugin plugin;
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

            plugin = new RedditTopCommentPlugin(unitTestContext.BotServicesMock.Object);

            plugin.BotName = unitTestContext.BotName;

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
            plugin.OnLoad();
        }

        [Test]
        public void RoastMe()
        {
            var msg = new GroupMeMessage();
            msg.text = "hey, roast me!";
            msg.name = "JohnDoe";

            bool handled =
            plugin.OnMessage(
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessageWithMention(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
