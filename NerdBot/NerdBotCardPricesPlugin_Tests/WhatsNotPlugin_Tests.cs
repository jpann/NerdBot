using System.Collections.Generic;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCardPrices;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NUnit.Framework;

namespace NerdBotCardPricesPlugin_Tests
{
    [TestFixture]
    public class WhatsNotPlugin_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private WhatsNotPlugin plugin;

        private UnitTestContext unitTestContext;

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
            plugin = new WhatsNotPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void WhatsNot()
        {
            // Setup ICardPriceStore mock
            unitTestContext.PriceStoreMock.Setup(ps => ps.GetCardsByPriceDecrease(It.IsAny<int>()))
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
                    unitTestContext.MessengerMock.Object
                    ).Result;

            unitTestContext.MessengerMock.Verify(m =>
                m.SendMessage("Today's Not-So-Hot Cards - Collected Company [DTK]: $9.24 down -106%, Dack Fayden [CNS]: $48.14 down -72%, Inkmoth Nexus [MBS]: $23.00 down -65%, Dimir Signet [ARC]: $0.70 down -46%, Elvish Archers [LEB]: $90.00 down -36%"),
                Times.Once);
        }
    }
}
