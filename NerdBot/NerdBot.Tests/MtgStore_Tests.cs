using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq;
using NerdBot.Mtg;
using NerdBot.TestsHelper;
using NerdBot.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests
{
    [TestFixture]
    class MtgStore_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private Mock<ILoggingService> loggingServiceMock;
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
        }

        [SetUp]
        public void SetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            mtgStore = new MtgStore(testConfig.Url, testConfig.Database, loggingServiceMock.Object, searchUtilityMock.Object);
        }

        #region CardExists
        [Test]
        public void CardExists_ByMultiverseId()
        {
            int multiverseId = 100;

            bool actual = mtgStore.CardExists(multiverseId).Result;

            Assert.True(actual);
        }

        [Test]
        public void CardExists_ByMultiverseId_DoesntExist()
        {
            int multiverseId = 9999999;

            bool actual = mtgStore.CardExists(multiverseId).Result;

            Assert.False(actual);
        }

        [Test]
        public void CardExists_ByName()
        {
            string name = "Spore Cloud";

            bool actual = mtgStore.CardExists(name).Result;

            Assert.True(actual);
        }

        [Test]
        public void CardExists_ByName_DoesntExist()
        {
            string name = "Blah Cloud";

            bool actual = mtgStore.CardExists(name).Result;

            Assert.False(actual);
        }

        [Test]
        public void CardExists_ByNameSet()
        {
            string name = "Spore Cloud";
            string set = "Masters Edition II";

            bool actual = mtgStore.CardExists(name, set).Result;

            Assert.True(actual);
        }

        [Test]
        public void CardExists_ByNameSet_DoesntExist()
        {
            string name = "Spore Cloud";
            string set = "Blah Edition II";

            bool actual = mtgStore.CardExists(name, set).Result;

            Assert.False(actual);
        }
        #endregion

        #region GetCard
        [Test]
        public void GetCard_ByName()
        {
            string name = "Spore Cloud";

            Card actual = mtgStore.GetCard(name).Result;

            Assert.NotNull(actual);
            Assert.AreEqual(name, actual.Name);
        }

        [Test]
        public void GetCard_ByName_DoesntExist()
        {
            string name = "Bore Cloud";

            Card actual = mtgStore.GetCard(name).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetCard_ByNameSet()
        {
            string name = "Spore Cloud";
            string set = "Masters Edition II";

            Card actual = mtgStore.GetCard(name, set).Result;

            Assert.NotNull(actual);
            Assert.AreEqual(name, actual.Name);
        }

        [Test]
        public void GetCard_ByNameSet_DoesntExist()
        {
            string name = "Spore Cloud";
            string set = "Blah Edition II";

            Card actual = mtgStore.GetCard(name, set).Result;

            Assert.Null(actual);
        }
        #endregion

        #region GetCards

        [Test]
        public void GetCards()
        {
            List<Card> cards = mtgStore.GetCards().Result;

            Assert.Greater(cards.Count, 1);
        }

        [Test]
        public void GetCards_ByName()
        {
            int expectedCount = 4;

            string name = "Spore Cloud";

            List<Card> cards = mtgStore.GetCards(name).Result;

            Assert.AreEqual(expectedCount, cards.Count);
        }

        [Test]
        public void GetCards_ByName_DoesntExist()
        {
            int expectedCount = 0;

            string name = "Bore Cloud";

            List<Card> cards = mtgStore.GetCards(name).Result;

            Assert.AreEqual(expectedCount, cards.Count);
        }
        #endregion

        #region GetCardOtherSets
        [Test]
        public void GetCardOtherSets()
        {
            int expectedCount = 2;
            string expectedOtherSet = "Masters Edition II";

            int multiverseId = 1922;

            List<Set> sets = mtgStore.GetCardOtherSets(multiverseId).Result;

            Assert.AreEqual(expectedCount, sets.Count);
        }

        [Test]
        public void GetCardOtherSets_DoesntExist()
        {
            int expectedCount = 0;

            int multiverseId = 999999;

            List<Set> sets = mtgStore.GetCardOtherSets(multiverseId).Result;

            Assert.AreEqual(expectedCount, sets.Count);
        }
        #endregion

        #region GetCardsBySet
        [Test]
        public void GetCardsBySet()
        {
            int expectedCount = 247;

            string name = "Masters Edition II";

            List<Card> cards = mtgStore.GetCardsBySet(name).Result;

            Assert.AreEqual(expectedCount, cards.Count);
        }

        [Test]
        public void GetCardsBySet_DoesntExist()
        {
            int expectedCount = 0;

            string name = "Blah Edition II";

            List<Card> cards = mtgStore.GetCardsBySet(name).Result;

            Assert.AreEqual(expectedCount, cards.Count);
        }
        #endregion

        #region SetExists
        [Test]
        public void SetExists_ByCode()
        {
            string code = "MMA";

            bool actual = mtgStore.SetExistsByCode(code).Result;

            Assert.True(actual);
        }

        [Test]
        public void SetExists_ByCode_DoesntExist()
        {
            string code = "XXX";

            bool actual = mtgStore.SetExistsByCode(code).Result;

            Assert.False(actual);
        }

        [Test]
        public void SetExists_ByName()
        {
            string name = "Masters Edition II";

            bool actual = mtgStore.SetExists(name).Result;

            Assert.True(actual);
        }

        [Test]
        public void SetExists_ByName_DoesntExist()
        {
            string name = "Blah Edition II";

            bool actual = mtgStore.SetExists(name).Result;

            Assert.False(actual);
        }
        #endregion

        #region GetSet

        [Test]
        public void GetSet()
        {
            string name = "Masters Edition II";

            Set actual = mtgStore.GetSet(name).Result;

            Assert.AreEqual(name, actual.Name);
        }

        [Test]
        public void GetSet_DoesntExist()
        {
            string name = "Blah Edition";

            Set actual = mtgStore.GetSet(name).Result;

            Assert.Null(actual);
        }

        [Test]
        public void GetSet_ByCode()
        {
            string code = "MMA";
            string name = "Modern Masters";

            Set actual = mtgStore.GetSetByCode(code).Result;

            Assert.NotNull(actual);
            Assert.AreEqual(name, actual.Name);
        }

        [Test]
        public void GetSet_ByCode_DoesntExist()
        {
            string code = "XXX";

            Set actual = mtgStore.GetSetByCode(code).Result;

            Assert.Null(actual);
        }
        #endregion

        #region GetRandomCardByArtist

        [Test]
        public void GetRandomCardByArtist()
        {
            string artist = "Scott Kirschner";

            Card actual = mtgStore.GetRandomCardByArtist(artist).Result;

            Assert.NotNull(actual);
            Assert.AreEqual(artist.ToLower(), actual.Artist.ToLower());
        }
        #endregion
    }
}
