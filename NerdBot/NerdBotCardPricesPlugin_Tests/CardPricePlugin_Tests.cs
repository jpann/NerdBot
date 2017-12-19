using Moq;
using NerdBot.TestsHelper;
using NerdBotCardPrices;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NUnit.Framework;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    public class CardPricePlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private CardPricePlugin plugin;

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

            var httpClient = new SimpleHttpClient(unitTestContext.LoggingServiceMock.Object);

            unitTestContext.BotServicesMock.SetupGet(b => b.HttpClient)
                .Returns(httpClient);

            plugin = new CardPricePlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void GetPrice_ByName()
        {
            string name = "Inquisition of Kozilek";

            // Setup mock to return price for this card by name
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name))
                .Returns(() => new CardPrice()
                {
                    Name = name,
                    SearchName = SearchHelper.GetSearchValue(name),
                    SetCode = "ROE",
                    PriceFoil = "$100",
                    PriceDiff = "10%"
                });

            var cmd = new Command()
            {
                Cmd = "tcg",
                Arguments = new string[]
                {
                    "Inquisition of Kozilek"
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => 
                m.SendMessage(It.Is<string>(s => s.StartsWith("Inquisition of Kozilek [ROE]"))),
                    Times.Once);
        }

        [Test]
        public void GetPrice_ByNameAndSet()
        {
            string name = "Inquisition of Kozilek";
            string setCode = "MD1";

            // Setup mock to return card price for card name and set
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name, setCode))
                .Returns(() => new CardPrice()
                {
                    Name = name,
                    SearchName = SearchHelper.GetSearchValue(name),
                    SetCode = "MD1",
                    PriceFoil = "$100",
                    PriceDiff = "10%"
                });

            var cmd = new Command()
            {
                Cmd = "tcg",
                Arguments = new string[]
                {
                    "MD1",
                    "Inquisition of Kozilek"
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => 
                m.SendMessage(It.Is<string>(s => s.StartsWith("Inquisition of Kozilek [MD1]"))),
                    Times.Once);
        }

        [Test]
        public void GetPrice_NoPriceFound_NoRecord()
        {
            string name = "Inquisition of Kozilek";
            string setCode = "MD1";

            // Setup mock to return card price for card name
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name, setCode))
                .Returns(() => null);

            var cmd = new Command()
            {
                Cmd = "tcg",
                Arguments = new string[]
                {
                    "Inquisition of Kozilek"
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m =>
                    m.SendMessage(It.Is<string>(s => s.StartsWith("Price unavailable"))),
                Times.Once);
        }

        [Test]
        public void GetPrice_NoPriceFound_RecordWithNoData()
        {
            string name = "Inquisition of Kozilek";
            string setCode = "MD1";

            // Setup mock to return card price for card name and set
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name))
                .Returns(() => new CardPrice()
                {
                    Name = name,
                    SearchName = SearchHelper.GetSearchValue(name),
                    SetCode = "ROE",
                    PriceLow = "",
                    PriceMid = "",
                    PriceFoil = "",
                    PriceDiff = ""
                });

            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name, setCode))
                .Returns(() => new CardPrice()
                {
                    Name = name,
                    SearchName = SearchHelper.GetSearchValue(name),
                    SetCode = "ROE",
                    PriceLow = "",
                    PriceMid = "",
                    PriceFoil = "",
                    PriceDiff = ""
                });

            var cmd = new Command()
            {
                Cmd = "tcg",
                Arguments = new string[]
                {
                    "Inquisition of Kozilek"
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m =>
                    m.SendMessage(It.Is<string>(s => s.StartsWith("Price unavailable"))),
                Times.Once);
        }

        [Test]
        public void GetPrice_SetNotAvailableFallbackToJustName()
        {
            string name = "Inquisition of Kozilek";
            string setCode = "MD1";

            // Setup mock to return NULL for this card and set
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name, setCode))
                .Returns(() => null);

            // Setup mock to return a price when falling back to just name
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardPrice(name))
                .Returns(() => new CardPrice()
                {
                    Name = name,
                    SearchName = SearchHelper.GetSearchValue(name),
                    SetCode = "ROE",
                    PriceFoil = "$100",
                    PriceDiff = "10%"
                });

            var cmd = new Command()
            {
                Cmd = "tcg",
                Arguments = new string[]
                {
                    "Inquisition of Kozilek"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled = plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                    ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Inquisition of Kozilek [ROE]"))),
                Times.Once);
        }

    }
}
