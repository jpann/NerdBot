using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Moq;
using MtgDb.Info.Driver;
using NerdBot.Utilities;
using NerdBot_DatabaseUpdater.Mappers;
using NUnit.Framework;

namespace NerdBot_DatabaseUpdater_Tests.Mappers
{
    [TestFixture]
    class MtgInfoMapper_Tests
    {
        private MtgDb.Info.Driver.Db mtgDb;
        private IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet> dataMapper;

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
            mtgDb = new Db();

            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));
        }

        [SetUp]
        public void SetUp()
        {
            dataMapper = new MtgInfoMapper(searchUtilityMock.Object);
        }

        [TearDown]
        public void TearDown()
        {

        }

        #region GetCard Tests
        [Test]
        public void GetCard()
        {
            
            
        }
        #endregion

        #region GetSet Tests
        #endregion
    }
}
