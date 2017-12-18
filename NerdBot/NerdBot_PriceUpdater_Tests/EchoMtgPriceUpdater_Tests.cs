using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Utilities;
using NerdBot_PriceUpdater.PriceUpdaters;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot_PriceUpdater_Tests
{
    [TestFixture]
    public class EchoMtgPriceUpdater_Tests
    {
        private IPriceUpdater priceUpdater;
        //private IHttpClient httpClient;
        private Mock<IHttpClient> httpClientMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Mock<SearchUtility> searchUtilityMock;

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
            // Setup mocks that dont change between tests
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();
            
            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetRegexSearchValue(s));

            
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICardPriceStore Mocks
            priceStoreMock = new Mock<ICardPriceStore>();

            priceStoreMock.Setup(p => p.FindAndModifySetPrice(It.IsAny<SetPrice>(), true))
                .Returns((SetPrice price, bool upsert) => price);

            priceStoreMock.Setup(p => p.FindAndModifyCardPrice(It.IsAny<CardPrice>(), true))
                .Returns((CardPrice price, bool upsert) => price);

            httpClientMock = new Mock<IHttpClient>();

            //httpClient = new SimpleHttpClient(loggingServiceMock.Object);

            priceUpdater = new EchoMtgPriceUpdater(
                priceStoreMock.Object,
                httpClientMock.Object,
                loggingServiceMock.Object,
                searchUtilityMock.Object);
        }

        [Test]
        public void UpdatePrices()
        {
            string url = "https://www.echomtg.com/set/HOU/";

            Set set = CreateSet();

            httpClientMock.Setup(h => h.GetPageSource(url))
                .Returns(ReadTestSetPage());

            priceUpdater.UpdatePrices(set);

            priceStoreMock.Verify(p => 
                p.FindAndModifySetPrice(
                    It.Is<SetPrice>(setPrice => 
                        setPrice.Name == set.Name &&
                        setPrice.SetCode == set.Code &&
                        setPrice.TotalCards == 204 &&
                        setPrice.SetValue == "$160.75" &&
                        setPrice.FoilSetValue == "$536" &&
                        setPrice.Url == url && 
                        setPrice.SearchName == set.SearchName), true), Times.Once);

            priceStoreMock.Verify(p =>
                p.FindAndModifyCardPrice(
                    It.Is<CardPrice>(card =>
                        card.SetCode == set.Code), true), Times.Exactly(204));
        }
    }
}
