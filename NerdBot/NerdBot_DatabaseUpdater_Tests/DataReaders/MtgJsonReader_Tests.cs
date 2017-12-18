using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Importer.DataReaders;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Importer.MtgData;
using NerdBotCommon.Mtg;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NerdBot_DatabaseUpdater_Tests.DataReaders
{
    [TestFixture]
    class MtgJsonReader_Tests
    {
        private MtgJsonReader reader;
        private UnitTestContext unitTestContext;

        private Mock<IMtgDataMapper<MtgJsonCard, MtgJsonSet>> dataMapperMock;
        private Mock<IFileSystem> fileSystemMock;

        private string jsonFileName = "C14-x_Truncated.json";

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
                SearchName = SearchHelper.GetSearchValue("Commander 2014"),
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
                    Colors = new List<string>() { "Black"},
                    Cost = "{2}{B}{B}",
                    Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
                    MultiverseId = 389422,
                    Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
                    Name = "Abyssal Persecutor",
                    SearchName = SearchHelper.GetSearchValue("Abyssal Persecutor"),
                    SetSearchName = SearchHelper.GetSearchValue("Commander 2014"),
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
                    Colors = new List<string>() { "White"},
                    Cost = "{4}{W}{W}",
                    Flavor = "She doesn't escort the dead to the afterlife, but instead raises them to fight and die again.",
                    MultiverseId = 389423,
                    Desc = "Flying, vigilance\n{T}: When target creature other than Adarkar Valkyrie dies this turn, return that card to the battlefield under your control.",
                    Name = "Adarkar Valkyrie",
                    Loyalty = "",
                    SearchName = SearchHelper.GetSearchValue("Adarkar Valkyrie"),
                    SetSearchName = SearchHelper.GetSearchValue("Commander 2014"),
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
        
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            unitTestContext = new UnitTestContext();

            // IMtgDataMapper Mock
            Card[] cards = GetTestCards();
            dataMapperMock = new Mock<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>();

            Set set = GetTestSet();

            dataMapperMock.Setup(
                d => d.GetCard(It.Is<MtgJsonCard>(c => c.Name == cards[0].Name), cards[0].SetName, cards[0].SetId))
                .Returns(() => cards[0]);

            dataMapperMock.Setup(
                d => d.GetCard(It.Is<MtgJsonCard>(c => c.Name == cards[1].Name), cards[1].SetName, cards[1].SetId))
                .Returns(() => cards[1]);

            dataMapperMock.Setup(
                d => d.GetSet(It.Is<MtgJsonSet>(s => s.Name == "Commander 2014")))
                .Returns(() => set);

            // IFileSystem Mock
            string fileName = Path.Combine(GetTestDataPath(), jsonFileName);
            fileSystemMock = new Mock<IFileSystem>();

            fileSystemMock.Setup(f => f.File.Exists(fileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.File.ReadAllText(fileName))
                .Returns(() => GetTestData());
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
                dataMapperMock.Object,
                unitTestContext.LoggingServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {

        }

        #region ReadCards Tests
        [Test]
        public void ReadCards()
        {
            var data = GetTestCards();

            var obj = new
            {
                name = data[0].SetName,
                code = data[0].SetId,
                cards = data
            };

            var cards_data = JsonConvert.SerializeObject(obj);
            var cards = reader.ReadCards(cards_data);

            List<Card> actualCards = cards.ToList();

            Assert.AreEqual(2, actualCards.Count());
            Assert.AreEqual(data[0].Name, actualCards[0].Name);
            Assert.AreEqual(data[1].Name, actualCards[1].Name);
        }
        #endregion

        #region ReadSet Tests
        [Test]
        public void ReadSet()
        {
            var data = GetTestCards();

            var obj = new
            {
                Name = "Commander 2014",
                SearchName = SearchHelper.GetSearchValue("Commander 2014"),
                BasicLandQty = 0,
                Block = "",
                Code = "C14",
                CommonQty = 0,
                Desc = "",
                MythicQty = 1,
                RareQty = 1,
                ReleasedOn = new DateTime(2014,11,7),
                Type = "commander",
                cards = data
            };

            var set_data = JsonConvert.SerializeObject(obj);

            var set = reader.ReadSet(set_data);

            Assert.AreEqual("Commander 2014", set.Name);
        }
        #endregion
    }
}
