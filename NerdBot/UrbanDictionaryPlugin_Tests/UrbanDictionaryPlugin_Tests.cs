using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotUrbanDictionary;
using NUnit.Framework;

namespace UrbanDictionaryPlugin_Tests
{
    [TestFixture]
    public class UrbanDictionaryPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private UrbanDictionaryPlugin plugin;
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

            plugin = new UrbanDictionaryPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void GetDefinition_IsA()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is a butt"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_IsAn()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is an butt"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_Is()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is butt"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_NoDefinition()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is sex pooping"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("There is no definition for that"), Times.AtLeastOnce);
        }
    }
}
