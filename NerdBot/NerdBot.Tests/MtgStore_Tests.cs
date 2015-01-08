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

        #endregion
    }
}
