using Moq;
using NerdBot;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBot.Utilities;
using NerdBotRedditTopCommentResponsePlugin;
using NUnit.Framework;
using SimpleLogging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NerdBot.TestsHelper;

namespace NerdBotRedditTopCommentResponsePlugin_Tests
{
    [TestFixture]
    public class RedditTopCommentPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private RedditTopCommentPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private IHttpClient httpClient;
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

            httpClient = new SimpleHttpClient(loggingServiceMock.Object);

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new RedditTopCommentPlugin(
                mtgStore,
                priceStoreMock.Object,
                commandParserMock.Object,
                httpClient,
                urlShortenerMock.Object);

            plugin.BotName = "TEST";

            plugin.LoggingService = loggingServiceMock.Object;
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
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessageWithMention(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
