using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Mtg;
using NUnit.Framework;

namespace NerdBot.Tests
{
    [TestFixture]
    class MtgStore_Tests
    {
        private Mock<DbSet<Card>> cardDbSetMock;
        private Mock<DbSet<Set>> setDbSetMock;
        private Mock<IMtgContext> contextMock ;
        private List<Card> cardData = new List<Card>();
        private List<Set> setData = new List<Set>(); 

        private void SetUp_Data()
        {
            cardData = new List<Card>()
            {
                new Card()
                {
                    Id = 5891,
                    RelatedCardId = 0,
                    SetNumber =  179,
                    Name = "Boros Charm",
                    SearchName = "Boros Charm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control gain indestructible until end of turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = "red;white",
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Commander 2013 Edition",
                    Type = "Instant",
                    SubType = "",
                    Power = "0",
                    Toughness = "00",
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
                    Id = 6975,
                    RelatedCardId = 0,
                    SetNumber =  137,
                    Name = "Boros Cluestone",
                    SearchName = "Boros Cluestone",
                    Desc = "{Tap}: Add {Red} or {White} to your mana pool.\n\r" +
                            "{Red}{White}, {Tap}, Sacrifice Boros Cluestone: Draw a card.",
                    Flavor = "",
                    Colors = "None",
                    Cost = "3",
                    Cmc = 3,
                    SetName = "Dragon's Maze",
                    Type = "Artifact",
                    SubType = "",
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
                    Id = 7213,
                    RelatedCardId = 0,
                    SetNumber =  148,
                    Name = "Boros Charm",
                    SearchName = "Boros Charm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control are indestructible this turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = "red;white",
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Gatecrash",
                    Type = "Instant",
                    SubType = "",
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
                    Name = "Spore Cloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    SetNumber = 176,
                    Cmc = 3,
                    Cost = "1GG",
                    SetName = "Masters Edition II",
                    SetId = "ME2",
                    Type = "Instant",
                    Img = "https://api.mtgdb.info/content/card_images/184710.jpeg",
                    MultiverseId = 184710
                },
                new Card()
                {
                    Name = "Spore Cloud",
                    Desc ="Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    SetNumber = 80,
                    Cmc = 3,
                    Cost = "1GG",
                    SetName = "Fallen Empires",
                    SetId = "FEM",
                    Type = "Instant",
                    Img = "https://api.mtgdb.info/content/card_images/1922.jpeg",
                    MultiverseId = 1922
                },
            };

            setData = new List<Set>()
            {
                new Set()
                {
                    Id = 116,
                    Name = "Gatecrash",
                    Code = "GTC",
                    Block = "Return to Ravnica",
                    Type = "Expansion",
                    Desc = "Gatecrash is a Magic: The Gathering expansion set released February 1, 2013. It is the second set of the Return to Ravnica block. The tagline for the set is 'Fight For Your Guild' and it contains 249 cards (101 commons, 80 uncommons, 53 rares, 15 mythic rares). Gatecrash focuses on five of the returning guilds; the Boros Legion, House Dimir, The Orzhov Syndicate, The Gruul Clans, and The Simic Combine. As in the original Ravnica block, Gatecrash focuses on multicolor cards. The storyline told deals with the rise of another faction that does not ally with any of the Guilds. This group is referred to as the 'Gateless'. The Gateless was referred to in the first set in certain cards as well. Another storyline has the tension between the guilds rise, and their attempts to thwart one another.",
                    CommonQty = 101,
                    UncommonQty = 80,
                    RareQty = 53,
                    MythicQty = 15,
                    BasicLandQty = 0,
                    TotalQty = 249,
                    ReleasedOn = new DateTime(2013,2,1)
                },
                new Set()
                {
                    Id = 124,
                    Name = "Commander 2013 Edition",
                    Code = "C13",
                    Block = "Commander",
                    Type = "Non-standard Legal",
                    Desc = "Commander is a series of five 100-card, three color Magic: the Gathering decks, meant as a supplement to the variant format initially known as 'Elder Dragon Highlander (EDH)'. Each deck is based around a legendary creature, called a 'Commander' or 'General'. No card other than basic lands appear more than once in each deck, and each deck contains three foil oversized legendary creature cards. This set is notable in that it is the first set printed outside of the normal booster pack expansions to have functionally new cards. There are 51 new cards, made specifically for multi-player games, featured in Commander.",
                    CommonQty = 28,
                    UncommonQty = 0,
                    RareQty = 0,
                    MythicQty = 0,
                    BasicLandQty = 0,
                    TotalQty = 28,
                    ReleasedOn = new DateTime(2013,11,1)
                },
            };
        }

        [SetUp]
        public void SetUp()
        {
            SetUp_Data();

            // Create a mock set and context
            cardDbSetMock = new Mock<DbSet<Card>>()
                .SetupData(cardData);

            setDbSetMock = new Mock<DbSet<Set>>()
                .SetupData(setData);

            contextMock = new Mock<IMtgContext>();
            contextMock.Setup(c => c.Cards).Returns(cardDbSetMock.Object);
            contextMock.Setup(c => c.Sets).Returns(setDbSetMock.Object);
        }

        #region CardExists
        [Test]
        public void CardExists_MultiverseId_True()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            int multiverseId = 376270;

            bool exists = mtgStore.CardExists(multiverseId);

            Assert.True(exists);
        }

        [Test]
        public void CardExists_MultiverseId_False()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            int multiverseId = 0;

            bool exists = mtgStore.CardExists(multiverseId);

            Assert.False(exists); 
        }

        [Test]
        public void CardExists_Name_True()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string name = "Boros Charm";

            bool exists = mtgStore.CardExists(name);

            Assert.True(exists);
        }

        [Test]
        public void CardExists_Name_False()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string name = "Some Random Card";

            bool exists = mtgStore.CardExists(name);

            Assert.False(exists);
        }

        [Test]
        public void CardExists_NameSet_True()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string name = "Boros Charm";
            string set = "Gatecrash";

            bool exists = mtgStore.CardExists(name, set);

            Assert.True(exists);
        }

        [Test]
        public void CardExists_NameSet_False()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string name = "Boros Charm";
            string set = "Random Set";

            bool exists = mtgStore.CardExists(name, set);

            Assert.False(exists);
        }
        #endregion

        #region GetCards
        [Test]
        public void Get_Cards()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.GetCards();

            Assert.AreEqual(cardData.Count, cards.Count());
        }

        [Test]
        public void Get_Cards_StartingWith_Boros_C()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.GetCards("Boros C");
            
            Assert.AreEqual(3, cards.Count());
        }

        [Test]
        public void Get_Cards_StartingWith_Coros()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.GetCards("Coros");

            Assert.AreEqual(0, cards.Count());
        }
        #endregion

        #region GetCard
        [Test]
        public void Get_Card()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.GetCard("Boros Charm");

            Assert.NotNull(card);
            Assert.AreEqual("Boros Charm", card.Name);
        }

        [Test]
        public void Get_Card_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.GetCard("Cluestone");

            Assert.Null(card);
        }

        [Test]
        public void Get_Card_Using_SetName()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.GetCard("Boros", "Commander 2013 Edition");

            Assert.NotNull(card);
            Assert.AreEqual("Boros Charm", card.Name);
            Assert.AreEqual("Commander 2013 Edition", card.SetName);
        }

        [Test]
        public void Get_Card_Using_SetName_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.GetCard("Boros", "xCommander 2013");

            Assert.Null(card);
        }

        [Test]
        public void Get_Card_Using_SetId_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.GetCard("Boros", "xC13");

            Assert.Null(card);
        }
        #endregion

        #region GetCardOtherSets
        [Test]
        public void Get_CardsOtherSets()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            int multiverseId = 376270;
            var otherSets = mtgStore.GetCardOtherSets(multiverseId);

            var otherSet = otherSets.First();

            Assert.AreEqual(1, otherSets.Count());
            Assert.AreEqual("Gatecrash", otherSet.Name);
        }
        #endregion
    }
}
