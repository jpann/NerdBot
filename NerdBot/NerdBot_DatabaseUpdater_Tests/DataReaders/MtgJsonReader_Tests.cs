using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Moq;
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
    class MtgJsonReader_Tests
    {
        private MtgJsonReader reader;

        private Mock<IMtgDataMapper<MtgJsonCard, MtgJsonSet>> dataMapperMock;
        private Mock<SearchUtility> searchUtilityMock;
        private Mock<IFileSystem> fileSystemMock;
        private Mock<ILoggingService> loggingServiceMock;

        private string jsonFileName = "C14_Truncated.json";

        #region Test Data Methods
        private string GetTestDataPath()
        {
            // Use the test assembly's directory instead of where nunit runs the test
            string outputPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");
            return Path.Combine(outputPath, "Data");
        }

        private string GetTestData()
        {
            string fileName = Path.Combine(GetTestDataPath(), jsonFileName);

            string data = File.ReadAllText(fileName);

            return data;
        }

        private Set GetTestSet()
        {
            Set set = new Set()
            {
                Name = "Commander 2014",
                SearchName = GetSearchValue("Commander 2014"),
                BasicLandQty = 0,
                Block = "",
                Code = "C14",
                CommonQty = 0,
                Desc = "",
                MythicQty = 1,
                RareQty = 1,
                ReleasedOn = new DateTime(2014,11,7),
                Type = "commander"
        };

            return set;
        }

        private Card[] GetTestCards()
        {
            Card[] cards = new Card[]
            {
                new Card()
                {
                    Artist = "Chippy",
                    Cmc = 4,
                    Colors = new string[] { "Black"},
                    Cost = "{2}{B}{B}",
                    Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
                    MultiverseId = 389422,
                    Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
                    Name = "Abyssal Persecutor",
                    SearchName = GetSearchValue("Abyssal Persecutor"),
                    SetSearchName = GetSearchValue("Commander 2014"),
                    SetId = "C14",
                    SetName = "Commander 2014",
                    Power = "6",
                    Toughness = "6",
                    Rarity = "Mythic Rare",
                    SubType = "Demon",
                    Token = false,
                    Type = "Creature",
                    Img = "http://mtgimage.com/multiverseid/389422.jpg",
                    ImgHires = "http://mtgimage.com/multiverseid/389422.jpg"
                },
                new Card()
                {
                    Artist = "Jeremy Jarvis",
                    Cmc = 6,
                    Colors = new string[] { "White"},
                    Cost = "{4}{W}{W}",
                    Flavor = "She doesn't escort the dead to the afterlife, but instead raises them to fight and die again.",
                    MultiverseId = 389423,
                    Desc = "Flying, vigilance\n{T}: When target creature other than Adarkar Valkyrie dies this turn, return that card to the battlefield under your control.",
                    Name = "Adarkar Valkyrie",
                    Loyalty = "",
                    SearchName = GetSearchValue("Adarkar Valkyrie"),
                    SetSearchName = GetSearchValue("Commander 2014"),
                    SetId = "C14",
                    SetName = "Commander 2014",
                    Power = "4",
                    Toughness = "5",
                    Rarity = "Rare",
                    SubType = "Angel",
                    Token = false,
                    Type = "Creature",
                    Img = "http://mtgimage.com/multiverseid/389423.jpg",
                    ImgHires = "http://mtgimage.com/multiverseid/389423.jpg"
                }

            };

            return cards;
        }
        #endregion

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

            // IMtgDataMapper Mock
            Card[] cards = GetTestCards();
            dataMapperMock = new Mock<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>();

            dataMapperMock.Setup(
                d => d.GetCard(It.Is<MtgJsonCard>(c => c.Name == "Abyssal Persecutor"), "Commander 2014", "C14"))
                .Returns(() => cards[0]);

            dataMapperMock.Setup(
                d => d.GetCard(It.Is<MtgJsonCard>(c => c.Name == "Adarkar Valkyrie"), "Commander 2014", "C14"))
                .Returns(() => cards[1]);

            // IFileSystem Mock
            string fileName = Path.Combine(GetTestDataPath(), jsonFileName);
            fileSystemMock = new Mock<IFileSystem>();

            fileSystemMock.Setup(f => f.File.Exists(fileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.File.ReadAllText(fileName))
                .Returns(() => GetTestData());

            // ILoggingService Mock
            loggingServiceMock = new Mock<ILoggingService>();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
            string fileName = Path.Combine(GetTestDataPath(), jsonFileName);

            reader = new MtgJsonReader(
                fileName,
                dataMapperMock.Object,
                fileSystemMock.Object,
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

            Assert.AreEqual(2, cards.Count());
        }
        #endregion
    }
}
