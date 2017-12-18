using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Importer.MtgData;
using NerdBotCommon.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NerdBot_DatabaseUpdater_Tests.Mappers
{
    [TestFixture]
    class MtgJsonMapper_Tests
    {
        private IMtgDataMapper<MtgJsonCard, MtgJsonSet> dataMapper;
        private Mock<SearchUtility> searchUtilityMock;

        private MtgJsonSet testJsonSet; // Contains the deserialized data from the mtg json file
        private List<MtgJsonCard> testJsonCards;
        private string testJsonFileName = "C14.json";
        
        private string GetTestDataPath()
        {
            string outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");
            return Path.Combine(outputPath, "Data");
        }

        private MtgJsonSet DeserailizeJsonSet(string fileName)
        {
            string data = File.ReadAllText(fileName);

            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            MtgJsonSet set = JsonConvert.DeserializeObject<MtgJsonSet>(data, settings);

            // Get rarity quantities
            JObject cardObject = JObject.Parse(data);
            IList<JToken> results = cardObject["cards"].Children().ToList();

            foreach (JToken result in results)
            {
                string rarity = result["rarity"].ToString();

                switch (rarity.ToLower())
                {
                    case "basic land":
                        set.BasicLandQty += 1;
                        break;
                    case "common":
                        set.CommonQty += 1;
                        break;
                    case "mythic":
                    case "mythic rare":
                        set.MythicQty += 1;
                        break;
                    case "uncommon":
                        set.UncommonQty += 1;
                        break;
                    case "rare":
                        set.RareQty += 1;
                        break;
                }
            }

            return set;
        }

        private List<MtgJsonCard> DeserializeJsonCards(string fileName)
        {
            string data = File.ReadAllText(fileName);

            JObject cardObject = JObject.Parse(data);

            IList<JToken> results = cardObject["cards"].Children().ToList();

            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            List<MtgJsonCard> cardData = new List<MtgJsonCard>();

            foreach (JToken result in results)
            {
                MtgJsonCard card = JsonConvert.DeserializeObject<MtgJsonCard>(result.ToString(), settings);
                cardData.Add(card);
            }

            return cardData;
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => SearchHelper.GetRegexSearchValue(s));

            // Deserialize test data
            string fileName = Path.Combine(GetTestDataPath(), testJsonFileName);
            testJsonSet = DeserailizeJsonSet(fileName);
            testJsonCards = DeserializeJsonCards(fileName);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            testJsonSet = null;
        }

        [SetUp]
        public void SetUp()
        {
            dataMapper = new MtgJsonMapper(searchUtilityMock.Object);

            dataMapper.ImageUrl = "http://localhost/%ID%.jpg";
            dataMapper.ImageHiResUrl = "http://localhost/%ID%.jpg";
        }

        [TearDown]
        public void TearDown()
        {

        }

        #region GetCard Tests
        [Test]
        public void GetCard()
        {
            var actual = dataMapper.GetCard(testJsonCards[0], testJsonSet.Name, testJsonSet.Code);

            Assert.AreEqual("Abyssal Persecutor", actual.Name);
            Assert.AreEqual(389422, actual.MultiverseId);
            Assert.AreEqual("Mythic Rare", actual.Rarity);
        }
        #endregion

        #region GetSet Tests

        [Test]
        public void GetSet()
        {
            var actual = dataMapper.GetSet(testJsonSet);

            Assert.AreEqual(testJsonSet.Name, actual.Name);
            Assert.AreEqual(testJsonSet.Code, actual.Code);
            Assert.AreEqual(testJsonSet.Block, actual.Block);
            Assert.AreEqual(testJsonSet.Type, actual.Type);
            Assert.AreEqual(testJsonSet.ReleaseDate, actual.ReleasedOn);
        }
        #endregion
    }
}
