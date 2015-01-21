using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NUnit.Framework;

namespace NerdBot.Tests
{
    [TestFixture]
    class MtgStore_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;

        public string GetSearchValue(string text)
        {
            string searchValue = text.ToLower();

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            searchValue = rgx.Replace(searchValue, "");
            searchValue = searchValue.Replace(" ", "");

            return searchValue;
        }

        [SetUp]
        public void SetUp()
        {
            mtgStore = new MtgStore(connectionString, databaseName);
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
            int expectedCount = 24152; // Count with FRF and C14 sets

            List<Card> cards = mtgStore.GetCards().Result;

            Assert.AreEqual(expectedCount, cards.Count);
        }

        [Test]
        public void GetCards_ByName()
        {
            int expectedCount = 2;

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
            int expectedCount = 1;
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
            int expectedCount = 245;

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
