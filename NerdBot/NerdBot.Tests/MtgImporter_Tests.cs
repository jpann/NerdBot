using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NerdBot.Importer;
using NerdBot.Mtg;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests
{
    [TestFixture]
    class MtgImporter_Tests
    {
        private Mock<IMtgStore> storeMock;
        private Mock<ILoggingService> loggerMock;

        private IImporter importer;
        private List<Card> cardData = new List<Card>();
        private List<Set> setData = new List<Set>();

        private void Setup_Data()
        {
            cardData = new List<Card>()
            {
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Charm",
                    SearchName = "boroscharm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control gain indestructible until end of turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "red", "white" },
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Commander 2013 Edition",
                    SetSearchName = "commander2013edition",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    SetId = "C13",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/376270.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/376270.jpg",
                    MultiverseId = 376270,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Charm",
                    SearchName = "boroscharm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control are indestructible this turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "red", "white" },
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Gatecrash",
                    SetSearchName = "gatecrash",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    SetId = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366435.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/366435.jpg",
                    MultiverseId = 366435,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Cluestone",
                    SearchName = "boroscluestone",
                    Desc = "{Tap}: Add {Red} or {White} to your mana pool.\n\r" +
                            "{Red}{White}, {Tap}, Sacrifice Boros Cluestone: Draw a card.",
                    Flavor = "",
                    Colors = new List<string>() { "none" },
                    Cost = "3",
                    Cmc = 3,
                    SetName = "Dragon's Maze",
                    SetSearchName = "dragonsmaze",
                    Type = "Artifact",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Raoul Vitale",
                    SetId = "DGM",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/368997.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/368997.jpg",
                    MultiverseId = 368997,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Spore Cloud",
                    SearchName = "sporecloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Flavor = "",
                    Colors = new List<string>() { "green" },
                    Cost = "1GG",
                    Cmc = 3,
                    SetName = "Masters Edition II",
                    SetSearchName = "masterseditionii",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Susan Van Camp",
                    SetId = "ME2",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/184710.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/184710.jpg",
                    MultiverseId = 184710,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Spore Cloud",
                    SearchName = "sporecloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Flavor = "",
                    Colors = new List<string>() { "green" },
                    Cost = "1GG",
                    Cmc = 3,
                    SetName = "Fallen Empires",
                    SetSearchName = "fallenempires",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Amy Weber",
                    SetId = "FEM",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/1922.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/1922.jpg",
                    MultiverseId = 1922,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Aerial Maneuver",
                    SearchName = "aerialmaneuver",
                    Desc = "Target creature gets +1/+1 and gains flying and first strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "white" },
                    Cost = "1W",
                    Cmc = 2,
                    SetName = "Gatecrash",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Scott Chou",
                    SetId = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366294.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/366294.jpg",
                    MultiverseId = 366294,
                },
            };

            setData = new List<Set>()
            {
                new Set()
                {
                    Name = "Gatecrash",
                    SearchName = "gatecrash",
                    Code = "GTC",
                    Block = "Return to Ravnica",
                    Type = "Expansion",
                    Desc = "Gatecrash is a Magic: The Gathering expansion set released February 1, 2013. It is the second set of the Return to Ravnica block. The tagline for the set is 'Fight For Your Guild' and it contains 249 cards (101 commons, 80 uncommons, 53 rares, 15 mythic rares). Gatecrash focuses on five of the returning guilds; the Boros Legion, House Dimir, The Orzhov Syndicate, The Gruul Clans, and The Simic Combine. As in the original Ravnica block, Gatecrash focuses on multicolor cards. The storyline told deals with the rise of another faction that does not ally with any of the Guilds. This group is referred to as the 'Gateless'. The Gateless was referred to in the first set in certain cards as well. Another storyline has the tension between the guilds rise, and their attempts to thwart one another.",
                    CommonQty = 101,
                    UncommonQty = 80,
                    RareQty = 53,
                    MythicQty = 15,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,2,1)
                },
                new Set()
                {
                    Name = "Commander 2013 Edition",
                    SearchName = "commander2013edition",
                    Code = "C13",
                    Block = "Commander",
                    Type = "Non-standard Legal",
                    Desc = "Commander is a series of five 100-card, three color Magic: the Gathering decks, meant as a supplement to the variant format initially known as 'Elder Dragon Highlander (EDH)'. Each deck is based around a legendary creature, called a 'Commander' or 'General'. No card other than basic lands appear more than once in each deck, and each deck contains three foil oversized legendary creature cards. This set is notable in that it is the first set printed outside of the normal booster pack expansions to have functionally new cards. There are 51 new cards, made specifically for multi-player games, featured in Commander.",
                    CommonQty = 28,
                    UncommonQty = 0,
                    RareQty = 0,
                    MythicQty = 0,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,11,1)
                },
                new Set()
                {
                    Name = "Dragon's Maze",
                    SearchName = "dragonsmaze",
                    Code = "DGM",
                    Block = "Return to Ravnica",
                    Type = "Expansion",
                    Desc = "Dragon's Maze is a Magic: The Gathering expansion set that was released on May 3, 2013. It is the third set of the Return to Ravnica block and contains 156 cards. All ten guilds of Ravnica will be included in the set. As in the original Ravnica block, Dragon's Maze focuses on multi-color cards. The set also marks the culmination of the Izzet League's research into the depths of Ravnica, the eponymous Dragon's Maze -- A path that treads all 10 guild gates in order to find and activate an energy source of immense power which is enough to subjugate and control all of the guilds. The Izzet propose a challenge in which each guild selects a champion in order to navigate and conquer the maze and subsequently the other guilds. Each Champion will have to traverse the maze, and they will have to deal with other guilds' attempts to halt their advance.",
                    CommonQty = 70,
                    UncommonQty = 40,
                    RareQty = 35,
                    MythicQty = 11,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,5,3)
                },
                new Set()
                {
                    Name = "Masters Edition II",
                    SearchName = "masterseditionii",
                    Code = "ME2",
                    Block = "Masters Editions",
                    Type = "Online",
                    Desc = @"The _Masters Edition II_ set is a collection of 
  _Magic_(TM) cards that were originally printed before the _Mirage_(TM) set was
  released. It's a 245-card, black-bordered set featuring 80 rares, 
  80 uncommons, 80 commons, and 5 basic lands. The _Masters Edition II_ set is 
  nonredeemable: Online cards from the _Masters Edition II_ set can't be 
  exchanged for physical cards",
                    CommonQty = 80,
                    UncommonQty = 80,
                    RareQty = 80,
                    MythicQty = 0,
                    BasicLandQty = 5,
                    ReleasedOn = new DateTime(2008,9,26)
                },
                new Set()
                {
                    Name = "Fallen Empires",
                    SearchName = "fallenempires",
                    Code = "FEM",
                    Block = null,
                    Type = "Expansion",
                    Desc = null,
                    CommonQty = 121,
                    UncommonQty = 30,
                    RareQty = 36,
                    MythicQty = 0,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(1994,1,11)
                },
            };
        }

        [SetUp]
        public void SetUp()
        {
            Setup_Data();

            loggerMock = new Mock<ILoggingService>();

            // Create a mock set and IMtgStore
            storeMock = new Mock<IMtgStore>();

            importer = new MtgImporter(storeMock.Object, loggerMock.Object);
        }

        [Test]
        public void Import()
        {
            var set = setData.Where(s => s.Code == "GTC").FirstOrDefault();
            var cards = cardData.Where(c => c.SetId == "GTC").AsEnumerable<Card>();

            storeMock.Setup(s => s.SetFindAndModify(set))
                .ReturnsAsync(set);

            storeMock.Setup(s => s.CardFindAndModify(It.IsAny<Card>()))
                .Returns((Card card) => Task.FromResult(card));

            var actual = importer.Import(set, cards).Result;

            Assert.NotNull(actual);
            Assert.AreEqual(actual.ImportedSet.Name, set.Name);
            Assert.AreEqual(actual.ImportedCards.Count, cards.Count());
            
        }
    }
}
