using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Moq;
using MtgDb.Info.Driver;
using NerdBot.Mtg;
using NerdBot.Utilities;
using NerdBot_DatabaseUpdater.DataReaders;
using NerdBot_DatabaseUpdater.Mappers;
using NerdBot_DatabaseUpdater.MtgData;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot_DatabaseUpdater_Tests.DataReaders
{
    [TestFixture]
    class MtgInfoReader_Tests
    {
        private MtgInfoReader reader;

        private IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet> dataMapper;
        private Mock<SearchUtility> searchUtilityMock;
        private Mock<ILoggingService> loggingServiceMock;
        private Db mtgInfoDb;

        private string setId = "FRF";

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
            // SearchUtility Mock
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            // ILoggingService Mock
            loggingServiceMock = new Mock<ILoggingService>();

            dataMapper = new MtgInfoMapper(searchUtilityMock.Object);

            mtgInfoDb = new Db();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
            reader = new MtgInfoReader(
                setId,
                mtgInfoDb,
                dataMapper,
                loggingServiceMock.Object);

        }

        [TearDown]
        public void TearDown()
        {

        }

        #region ReadCards Tests
        [Test]
        public void ReadCards()
        {
            var cards = reader.ReadCards();

            List<Card> actualCards = cards.ToList();

            Assert.AreEqual(184, actualCards.Count());
            Assert.AreEqual("Ugin, the Spirit Dragon", actualCards[0].Name);
            Assert.AreEqual("Abzan Advantage", actualCards[1].Name);
        }
        #endregion

        #region ReadSet Tests
        [Test]
        public void ReadSet()
        {
            var set = reader.ReadSet();

            Assert.AreEqual("Fate Reforged", set.Name);
        }
        #endregion
    }
}
