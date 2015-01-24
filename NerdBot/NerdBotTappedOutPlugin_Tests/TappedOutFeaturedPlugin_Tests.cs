using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBotTappedOut;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotTappedOutPlugin_Tests
{
    [TestFixture]
    class TappedOutFeaturedPlugin_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private TappedOutFeaturedPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;

        private string featuredJson = @"[
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
            loggingServiceMock = new Mock<ILoggingService>();

            mtgStore = new MtgStore(connectionString, databaseName, loggingServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICommandParser Mocks
            commandParserMock = new Mock<ICommandParser>();

            // Setup IHttpClient Mocks
            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(featuredJson);

            httpClientMock = new Mock<IHttpClient>();

            httpClientMock.Setup(h => h.GetAsJson("http://tappedout.net/api/deck/latest/featured/"))
                .Returns(httpJsonTask.Task);

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            urlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/22-01-15-deck-1/"))
                .Returns("http://deck1");

            urlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/deck-2/"))
                .Returns("http://deck2");

            urlShortenerMock.Setup(u => u.ShortenUrl("http://tappedout.net/mtg-decks/21-01-15-deck-3-esper/"))
                .Returns("http://deck3");

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new TappedOutFeaturedPlugin(
                mtgStore,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);
        }

        [Test]
        public void LatestFeatured()
        {
            var cmd = new Command()
            {
                Cmd = "featured",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Featured decks: Deck 1 [http://deck1], Deck 2 [http://deck2], Deck 3 (Esper) [http://deck3] [3/3]")));
        }

        [Test]
        public void LatestFeatured_NameContains()
        {
            var cmd = new Command()
            {
                Cmd = "featured",
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
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Featured decks: Deck 3 (Esper) [http://deck3] [1/3]")));
        }

        [Test]
        public void LatestFeatured_NameContains_Empty()
        {
            var cmd = new Command()
            {
                Cmd = "featured",
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
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == "Featured decks:  [0/3]")));
        }
    }
}
