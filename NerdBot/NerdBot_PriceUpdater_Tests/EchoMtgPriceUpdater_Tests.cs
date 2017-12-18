using System;
using System.IO;
using System.Reflection;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBot_PriceUpdater.PriceUpdaters;
using NUnit.Framework;

namespace NerdBot_PriceUpdater_Tests
{
    [TestFixture]
    public class EchoMtgPriceUpdater_Tests
    {
        private IPriceUpdater priceUpdater;
        private UnitTestContext unitTestContext;

        #region Test Utility Methods
        private Set CreateSet()
        {
            var set = new Set()
            {
                Name = "Hour of Devastation",
                SearchName = "hourofdevastation",
                Block = "Amonkhet",
                Type = "expansion",
                Code = "HOU",
                CommonQty = 74,
                UncommonQty = 67,
                RareQty = 49,
                MythicQty = 14,
                BasicLandQty = 15,
                ReleasedOn = new DateTime(2017, 7, 14),
                MagicCardsInfoCode = "hou"
            };

            return set;
        }

        private string ReadTestSetPage()
        {
            string testSetFile = Path.Combine(Path.GetDirectoryName(AssemblyLocation()), "TestSet.html");

            string source = File.ReadAllText(testSetFile); 

            return source;
        }

        private string AssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = codebase.LocalPath;
            return path;
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            // Setup ICardPriceStore Mocks
            unitTestContext.PriceStoreMock.Setup(p => p.FindAndModifySetPrice(It.IsAny<SetPrice>(), true))
                .Returns((SetPrice price, bool upsert) => price);

            unitTestContext.PriceStoreMock.Setup(p => p.FindAndModifyCardPrice(It.IsAny<CardPrice>(), true))
                .Returns((CardPrice price, bool upsert) => price);

            priceUpdater = new EchoMtgPriceUpdater(
                unitTestContext.PriceStoreMock.Object,
                unitTestContext.HttpClientMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);
        }

        [Test]
        public void UpdatePrices()
        {
            string url = "https://www.echomtg.com/set/HOU/";

            Set set = CreateSet();

            unitTestContext.HttpClientMock.Setup(h => h.GetPageSource(url))
                .Returns(ReadTestSetPage());

            priceUpdater.UpdatePrices(set);

            unitTestContext.PriceStoreMock.Verify(p => 
                p.FindAndModifySetPrice(
                    It.Is<SetPrice>(setPrice => 
                        setPrice.Name == set.Name &&
                        setPrice.SetCode == set.Code &&
                        setPrice.TotalCards == 204 &&
                        setPrice.SetValue == "$160.75" &&
                        setPrice.FoilSetValue == "$536" &&
                        setPrice.Url == url && 
                        setPrice.SearchName == set.SearchName), true), Times.Once);

            unitTestContext.PriceStoreMock.Verify(p =>
                p.FindAndModifyCardPrice(
                    It.Is<CardPrice>(card =>
                        card.SetCode == set.Code), true), Times.Exactly(204));
        }
    }
}
