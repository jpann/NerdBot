using System.Threading.Tasks;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotTappedOut;
using NUnit.Framework;

namespace NerdBotTappedOutPlugin_Tests
{
    [TestFixture]
    class TappedOutLatestTop8Plugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private TappedOutLatestTop8Plugin plugin;

        private UnitTestContext unitTestContext;

        private string tappedOutJson = @"[
   {
      ""user_display"":""user1"",
      ""name"":""Deck 1"",
      ""url"":""http://tappedout.net/mtg-decks/22-01-15-deck-1/"",
      ""user"":""user1"",
      ""slug"":""22-01-15-deck-1"",
      ""resource_uri"":""http://tappedout.net/api/collection/collection:deck/22-01-15-deck-1/""
   },
   {
      ""user_display"":""user2"",
      ""name"":""Deck 2"",
      ""url"":""http://tappedout.net/mtg-decks/deck-2/"",
      ""user"":""user2"",
      ""slug"":""deck-2"",
      ""resource_uri"":""http://tappedout.net/api/collection/collection:deck/deck-2/""
   },
   {
      ""user_display"":""user3"",
      ""name"":""Deck 3 (Esper)"",
      ""url"":""http://tappedout.net/mtg-decks/21-01-15-deck-3-esper/"",
      ""user"":""user3"",
      ""slug"":""21-01-15-deck-3-esper"",
      ""resource_uri"":""http://tappedout.net/api/collection/collection:deck/21-01-15-deck-3-esper/""
   }
]";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            unitTestContext = new UnitTestContext();

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.Database,
                unitTestContext.QueryStatisticsStoreMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup IHttpClient Mocks
            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/top8-latest/"))
                .Returns(httpJsonTask.Task);

            // Setup IUrlShortener Mocks
            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/22-01-15-deck-1/"))
                .Returns("http://deck1");

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/deck-2/"))
                .Returns("http://deck2");

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/21-01-15-deck-3-esper/"))
                .Returns("http://deck3");
            
            plugin = new TappedOutLatestTop8Plugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void LatestTop8()
        {
            var cmd = new Command()
            {
                Cmd = "top8",
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

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Top-8 decks: Deck 1 [http://deck1], Deck 2 [http://deck2], Deck 3 (Esper) [http://deck3] [3/3]")));
        }

        [Test]
        public void LatestTop8_NameContains()
        {
            var cmd = new Command()
            {
                Cmd = "top8",
                Arguments = new string[]
                {
                    "esper"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Top-8 decks: Deck 3 (Esper) [http://deck3] [1/3]")));
        }

        [Test]
        public void LatestTop8_NameContains_Empty()
        {
            var cmd = new Command()
            {
                Cmd = "top8",
                Arguments = new string[]
                {
                    "xxxx"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Top-8 decks:  [0/3]")));
        }
    }
}
