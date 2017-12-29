using System.Collections.Generic;
using NerdBot.TestsHelper;
using NerdBotCommon.Autocomplete;
using NerdBotCommon.ThirdParty.ScryFall;
using NUnit.Framework;
using Moq;

namespace NerdBot.Tests.ThirdParty.ScryFall
{
    [TestFixture]
    public class ScryFallAutocomplete_Tests
    {
        private IAutocompleter autocompleter;

        private UnitTestContext unitTestContext;

        private string scryFallData;
        private string scryFallNoData;

        #region Setup Test Data

        private void SetupData()
        {
            scryFallData = @"{
object: ""catalog"",
total_items: 16,
data: [
""Spore Frog"",
""Spore Burst"",
""Sporemound"",
""Spore Cloud"",
""Spore Flower"",
""Sporeback Troll"",
""Sporecap Spider"",
""Sporesower Thallid"",
""Plague Spores"",
""Rustspore Ram"",
""Martyr of Spores"",
""Vitaspore Thallid"",
""Mindbender Spores"",
""Ghave, Guru of Spores"",
""Deathspore Thallid"",
""Bloodspore Thrinax""
]
}";

            scryFallNoData = @"{
object: ""catalog"",
total_items: 0,
data: [ ]
}";
        }
        #endregion

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            unitTestContext = new UnitTestContext();
            
            autocompleter = new ScryFallAutocomplete(unitTestContext.HttpClientMock.Object, unitTestContext.LoggingServiceMock.Object);

            SetupData();
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void Get_WithResults()
        {
            string term = "spore";

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("https://api.scryfall.com/cards/autocomplete?q=" + term))
                .ReturnsAsync(scryFallData);

            List<string> actual = autocompleter.GetAutocompleteAsync(term).Result;

            Assert.AreEqual(16, actual.Count);
        }

        [Test]
        public void Get_WithNoResults()
        {
            string term = "x";

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("https://api.scryfall.com/cards/autocomplete?q=" + term))
                .ReturnsAsync(scryFallNoData);

            List<string> actual = autocompleter.GetAutocompleteAsync(term).Result;

            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void Get_WithNullResults()
        {
            string term = "null";

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("https://api.scryfall.com/cards/autocomplete?q=" + term))
                .ReturnsAsync(() => null);

            List<string> actual = autocompleter.GetAutocompleteAsync(term).Result;

            Assert.IsNull(actual);
        }
    }
}