using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot.Mtg.Prices;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests.PriceStore
{
    [TestFixture]
    class EchoMtgPriceStore_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb_prices";

        private ICardPriceStore priceStore;
        private Mock<ILoggingService> loggingServiceMock;

        private string GetSearchValue(string text, bool forRegex = false)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.^*]");

            string searchValue = text.ToLower();

            if (forRegex)
            {
                // Replace * and % with a regex '*' char
                searchValue = searchValue.Replace("%", ".*");

                // If the first character of the searchValue is not '*', 
                // meaning the user does not want to do a contains search,
                // explicitly use a starts with regex
                if (!searchValue.StartsWith(".*"))
                {
                    searchValue = "^" + searchValue;
                }
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

            priceStore = new EchoMtgPriceStore(connectionString, databaseName, loggingServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        #region GetCardPrice
        [Test]
        public void GetCardPrice_ByName()
        {
            string name = "Goblin War Paint";

            CardPrice actual = priceStore.GetCardPrice(name);

            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual("https://www.echomtg.com/card/79653/goblin-war-paint/", actual.Url);
        }

        [Test]
        public void GetCardPrice_ByName_DoesntExist()
        {
            string name = "Goblin War XXXX";

            CardPrice actual = priceStore.GetCardPrice(name);

            Assert.Null(actual);
        }

        [Test]
        public void GetCardPrice_ByNameSet()
        {
            string name = "Goblin War Paint";
            string setCode = "ZEN";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual("https://www.echomtg.com/card/58811/goblin-war-paint/", actual.Url);
        }

        [Test]
        public void GetCardPrice_ByNameSet_NameDoesntExist()
        {
            string name = "Goblin War XXXX";
            string setCode = "ZEN";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.Null(actual);
        }

        [Test]
        public void GetCardPrice_ByNameSet_SetDoesntExist()
        {
            string name = "Goblin War Paint";
            string setCode = "XXX";

            CardPrice actual = priceStore.GetCardPrice(name, setCode);

            Assert.Null(actual);
        }
        #endregion

        #region InsertCardPrice
        #endregion

        #region RemoveCardPrice
        #endregion

        #region FindAndModifyCardPrice

        #endregion
    }
}
