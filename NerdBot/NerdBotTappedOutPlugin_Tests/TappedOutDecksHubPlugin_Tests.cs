using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot;
using NerdBot.Parsers;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using NerdBotTappedOut;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotTappedOutPlugin_Tests
{
    [TestFixture]
    class TappedOutDecksHubPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private TappedOutDecksHubPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<SearchUtility> searchUtilityMock;

        public string GetSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        public string GetRegexSearchValue(string text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            // Replace * and % with a regex '*' char
            searchValue = searchValue.Replace("%", ".*");

            // If the first character of the searchValue is not '*', 
            // meaning the user does not want to do a contains search,
            // explicitly use a starts with regex
            if (!searchValue.StartsWith(".*"))
            {
                searchValue = "^" + searchValue;
            }

            // Remove all non a-zA-Z0-9.^ characters
            searchValue = rgx.Replace(searchValue, "");

            // Remove all spaces
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }


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

            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.Database,
                loggingServiceMock.Object,
                searchUtilityMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICardPriceStore Mocks
            priceStoreMock = new Mock<ICardPriceStore>();

            // Setup ICommandParser Mocks
            commandParserMock = new Mock<ICommandParser>();

            // Setup IHttpClient Mocks
            httpClientMock = new Mock<IHttpClient>();

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

            plugin = new TappedOutDecksHubPlugin(
                mtgStore,
                priceStoreMock.Object,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object,
                new BotConfig());

            plugin.LoggingService = loggingServiceMock.Object;
        }

        [Test]
        public void Decks_BySlug()
        {
            string slug = "tribal";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson);

            httpClientMock.Setup(h => h.GetAsJson(string.Format("http://tappedout.net/api/deck/latest/{0}/", slug)))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "decks",
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
                    messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == slug + " decks: Deck 1 [http://deck1], Deck 2 [http://deck2], Deck 3 (Esper) [http://deck3] [3/3]")));
        }
        
        [Test]
        public void Decks_BySlug_Filter()
        {
            string slug = "tribal";
            string filter = "esper";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson);

            httpClientMock.Setup(h => h.GetAsJson(string.Format("http://tappedout.net/api/deck/latest/{0}/", slug)))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "decks",
                Arguments = new string[]
                {
                    slug,
                    filter
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
                It.Is<string>(s => s == slug + " decks: Deck 3 (Esper) [http://deck3] [1/3]")));
        }

        [Test]
        public void Decks_EmptyResponse()
        {
            string slug = "vampires";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(tappedOutJson_NoData);

            httpClientMock.Setup(h => h.GetAsJson(string.Format("http://tappedout.net/api/deck/latest/{0}/", slug)))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "decks",
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
                    messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == string.Format("No deck hub found for '{0}'... Check http://tappedout.net/mtg-deck-builder/ for the hub name.", slug))));
        }

        [Test]
        public void Decks_NullResponse()
        {
            string slug = "vampires";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(null);

            httpClientMock.Setup(h => h.GetAsJson(string.Format("http://tappedout.net/api/deck/latest/{0}/", slug)))
                .Returns(httpJsonTask.Task);

            var cmd = new Command()
            {
                Cmd = "decks",
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
                    messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(
                It.Is<string>(s => s == string.Format("No deck hub found for '{0}'... Check http://tappedout.net/mtg-deck-builder/ for the hub name.", slug))));
        }
    }
}
