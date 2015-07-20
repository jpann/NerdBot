using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBot.Utilities;
using NerdBotCardPrices;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    public class WhatsNotPlugin_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private WhatsNotPlugin plugin;

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

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            mtgStore = new MtgStore(
                connectionString,
                databaseName,
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

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new WhatsNotPlugin(
                mtgStore,
                priceStoreMock.Object,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);

            plugin.LoggingService = loggingServiceMock.Object;
        }

        [Test]
        public void WhatsNot()
        {
            // Setup ICardPriceStore mock
            priceStoreMock.Setup(ps => ps.GetCardsByPriceDecrease(It.IsAny<int>()))
                .Returns(() => new List<CardPrice>()
                {
                    new CardPrice()
                    {
                        Name = "Collected Company",
                        SearchName = "collectedcompany",
                        SetCode = "DTK",
                        PriceDiff = "-106%",
                        PriceDiffValue = -106,
                        PriceMid = "$9.24",
                        PriceLow = "$7.61",
                        PriceFoil = "$31.50"
                    },
                    new CardPrice()
                    {
                        Name = "Dack Fayden",
                        SearchName = "dackfayden",
                        SetCode = "CNS",
                        PriceDiff = "-72%",
                        PriceDiffValue = -72,
                        PriceMid = "$48.14",
                        PriceLow = "$34.99",
                        PriceFoil = "$329.82"
                    },
                    new CardPrice()
                    {
                        Name = "Inkmoth Nexus",
                        SearchName = "inkmothnexus",
                        SetCode = "MBS",
                        PriceDiff = "-65%",
                        PriceDiffValue = -65,
                        PriceMid = "$23.00",
                        PriceLow = "$18.50",
                        PriceFoil = "$57.70",
                    },
                    new CardPrice()
                    {
                        Name = "Dimir Signet",
                        SearchName = "dimirsignet",
                        SetCode = "ARC",
                        PriceDiff = "-46%",
                        PriceDiffValue = -46,
                        PriceMid = "$0.70",
                        PriceLow = "$0.38",
                        PriceFoil = "",
                    },
                    new CardPrice()
                    {
                        Name = "Elvish Archers",
                        SearchName = "elvisharchers",
                        SetCode = "LEB",
                        PriceDiff = "-36%",
                        PriceDiffValue = -36,
                        PriceMid = "$90.00",
                        PriceLow = "$90.00",
                        PriceFoil = "",
                    },
                });

            var cmd = new Command()
            {
                Cmd = "whatsnot"
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                    cmd,
                    msg,
                    messengerMock.Object
                    ).Result;

            messengerMock.Verify(m =>
                m.SendMessage("Today's Not-So-Hot Cards - Collected Company [DTK]: $9.24 down -106%, Dack Fayden [CNS]: $48.14 down -72%, Inkmoth Nexus [MBS]: $23.00 down -65%, Dimir Signet [ARC]: $0.70 down -46%, Elvish Archers [LEB]: $90.00 down -36%"),
                Times.Once);
        }
    }
}
