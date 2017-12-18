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
    class TappedOutComboDecksPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private TappedOutComboDecksPlugin plugin;

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
      ""slug"":""vampire-tribal"",
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

        private string tappedOutJson_NoData = @"[
]";

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

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/22-01-15-deck-1/"))
                .Returns("http://deck1");

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/deck-2/"))
                .Returns("http://deck2");

            unitTestContext.UrlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/21-01-15-deck-3-esper/"))
                .Returns("http://deck3");
            
            plugin = new TappedOutComboDecksPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void ComboDecks()
        {
            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/combo/"))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "combodecks",
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
                It.Is<string>(s => s == "Combo decks: Deck 1 [http://deck1], Deck 2 [http://deck2], Deck 3 (Esper) [http://deck3] [3/3]")));
        }

        [Test]
        public void ComboDecks_NameContains()
        {
            string slug = "esper";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/combo/"))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "combodecks",
                Arguments = new string[]
                {
                    slug
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
                It.Is<string>(s => s == "Combo decks: Deck 3 (Esper) [http://deck3] [1/3]")));
        }

        [Test]
        public void ComboDecks_EmptyResponse()
        {
            string slug = "vampires";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson_NoData);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/combo/"))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "combodecks",
                Arguments = new string[]
                {
                    slug
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
                It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ComboDecks_NullResponse()
        {
            string slug = "vampires";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(null);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/combo/"))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "combodecks",
                Arguments = new string[]
                {
                    slug
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
                It.IsAny<string>()), Times.Never);
        }

    }
}
