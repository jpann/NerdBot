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
                    Related_Card_Id = 0,
                    Set_Number =  179,
                    Name = "Boros Charm",
                    Search_Name = "Boros Charm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control gain indestructible until end of turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = "red;white",
                    Cost = "RW",
                    Cmc = 2,
                    Set_Name = "Commander 2013 Edition",
                    Type = "Instant",
                    SubType = "",
                    Power = "0",
                    Toughness = "00",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    Set_Id = "C13",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/376270.jpeg",
                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/376270.jpg",
                    Multiverse_Id = 376270,
                },
                new Card()
                {
                    Id = 6975,
                    Related_Card_Id = 0,
                    Set_Number =  137,
                    Name = "Boros Cluestone",
                    Search_Name = "Boros Cluestone",
                    Desc = "{Tap}: Add {Red} or {White} to your mana pool.\n\r" +
                            "{Red}{White}, {Tap}, Sacrifice Boros Cluestone: Draw a card.",
                    Flavor = "",
                    Colors = "None",
                    Cost = "3",
                    Cmc = 3,
                    Set_Name = "Dragon's Maze",
                    Type = "Artifact",
                    SubType = "",
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Raoul Vitale",
                    Set_Id = "DGM",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/368997.jpeg",
                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/368997.jpg",
                    Multiverse_Id = 368997,
                },
                new Card()
                {
                    Id = 7213,
                    Related_Card_Id = 0,
                    Set_Number =  148,
                    Name = "Boros Charm",
                    Search_Name = "Boros Charm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control are indestructible this turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = "red;white",
                    Cost = "RW",
                    Cmc = 2,
                    Set_Name = "Gatecrash",
                    Type = "Instant",
                    SubType = "",
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    Set_Id = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366435.jpeg",
                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/366435.jpg",
                    Multiverse_Id = 366435,
                },
                new Card()
                {
                    Name = "Spore Cloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Set_Number = 176,
                    Cmc = 3,
                    Cost = "1GG",
                    Set_Name = "Masters Edition II",
                    Set_Id = "ME2",
                    Type = "Instant",
                    Img = "https://api.mtgdb.info/content/card_images/184710.jpeg",
                    Multiverse_Id = 184710
                },
                new Card()
                {
                    Name = "Spore Cloud",
                    Desc ="Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Set_Number = 80,
                    Cmc = 3,
                    Cost = "1GG",
                    Set_Name = "Fallen Empires",
                    Set_Id = "FEM",
                    Type = "Instant",
                    Img = "https://api.mtgdb.info/content/card_images/1922.jpeg",
                    Multiverse_Id = 1922
                },
                new Card()
                {
                    Id = 7066,
                    Related_Card_Id = 0,
                    Set_Number =  1,
                    Name = "Aerial Maneuver",
                    Search_Name = "Aerial Maneuver",
                    Desc = "Target creature gets +1/+1 and gains flying and first strike until end of turn.",
                    Flavor = "",
                    Colors = "white",
                    Cost = "1W",
                    Cmc = 2,
                    Set_Name = "Gatecrash",
                    Type = "Instant",
                    SubType = "",
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Scott Chou",
                    Set_Id = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366294.jpeg",
                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/366294.jpg",
                    Multiverse_Id = 366294,
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

            // Mock querying for a card that start with 'boros' using a like query
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 LIMIT 1", new string[] {"boros%"}))
                .Returns(() => cardData.Where(c => c.Name.ToLower().StartsWith("boros")).ToList());

            // Mock querying for a card that start with 'doesntexist' which will return an empty list
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 LIMIT 1", new string[] { "doesntexist%" }))
                .Returns(() => new List<Card>());

            // Mock querying for a card that starts with 'boros' and is in set 'Commander 2013 Edition'
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 AND set_name LIKE @p1 LIMIT 1", new string[] { "boros%" , "commander 2013 edition" }))
                .Returns(() => cardData.Where(c => c.Name.ToLower().StartsWith("boros") && c.Set_Name == "Commander 2013 Edition").ToList());

            // Mock querying for a card that starts with 'doesntexist' and is in set 'Commander 2013 Edition' which will return an empty list
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 AND set_name LIKE @p1 LIMIT 1", new string[] { "doesntexist%", "commander 2013 edition" }))
                .Returns(() => new List<Card>());

            // Mock querying for cards that start with 'boros'
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0", new string[] { "boros%" }))
                .Returns(() => cardData.Where(c => c.Name.ToLower().StartsWith("boros")).ToList());

            // Mock querying for cards that start with 'doesntexist' which will return an empty list
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0", new string[] { "doesntexist%" }))
                .Returns(() => new List<Card>());

            // Mock querying for cards that start with 'boros' and set contains 'c'
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 AND set_name LIKE @p1", new string[] { "boros%", "%c%" }))
                .Returns(() => cardData.Where(c => c.Name.ToLower().StartsWith("boros")&& c.Set_Name.ToLower().Contains("c")).ToList());

            // Mock querying for cards that start with 'boros' and set contains 'DOESNTEXIST%' which will return an empty list
            contextMock.Setup(
                c => c.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0 AND set_name LIKE @p1", new string[] { "boros%", "doesntexist%" }))
                .Returns(() => new List<Card>());
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
            Assert.AreEqual("Commander 2013 Edition", card.Set_Name);
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

        #region SearchCard
        [Test]
        public void SearchCard()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.SearchCard("Boros%");

            Assert.NotNull(card);
            Assert.AreEqual(cardData.Where(c => c.Name.ToLower().StartsWith("boros")).ToList().FirstOrDefault().Multiverse_Id, 
                card.Multiverse_Id);
        }

        [Test]
        public void SearchCard_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.SearchCard("DOESNTEXIST%");

            Assert.Null(card);
        }

        [Test]
        public void SearchCard_SetName()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.SearchCard("Boros%", "Commander 2013 Edition");

            Assert.NotNull(card);
            Assert.AreEqual(cardData.Where(c => c.Name.ToLower().StartsWith("boros") && c.Set_Name == "Commander 2013 Edition").ToList().FirstOrDefault().Multiverse_Id,
                card.Multiverse_Id);
        }

        [Test]
        public void SearchCard_SetName_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var card = mtgStore.SearchCard("DOESNTEXIST%", "Commander 2013 Edition");

            Assert.Null(card);
        }
        #endregion

        #region SearchCards
        [Test]
        public void SearchCards()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.SearchCards("Boros%");

            Assert.AreEqual(cardData.Where(c => c.Name.ToLower().StartsWith("boros")).Count(), cards.Count);
        }

        [Test]
        public void SearchCards_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.SearchCards("DOESNTEXIST%");

            Assert.AreEqual(0, cards.Count);
        }

        [Test]
        public void SearchCards_SetName()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.SearchCards("Boros%", "%C%");

            Assert.AreEqual(cardData.Where(c => c.Name.ToLower().StartsWith("boros") && c.Set_Name.ToLower().Contains("c")).Count(), cards.Count);
        }

        [Test]
        public void SearchCards_SetName_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            var cards = mtgStore.SearchCards("Boros%", "DOESNTEXIST%");

            Assert.AreEqual(0, cards.Count);
        }
        #endregion

        #region #region GetCardOtherSets
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

        [Test]
        public void Get_CardsOtherSets_NoOtherSets()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            int multiverseId = 368997;
            var otherSets = mtgStore.GetCardOtherSets(multiverseId);

            Assert.AreEqual(0, otherSets.Count());
        }

        [Test]
        public void Get_CardsOtherSets_CardDoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            int multiverseId = 0;
            
            Exception ex = Assert.Throws<Exception>(
            delegate
            {
                var otherSets = mtgStore.GetCardOtherSets(multiverseId);
            });

            Assert.That(ex.Message, Is.EqualTo(string.Format("No card exists using multiverse id of '{0}'.", multiverseId)));
        }
        #endregion

        #region GetCardsBySet
        [Test]
        public void GetCardsBySet()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "Gatecrash";

            var cards = mtgStore.GetCardsBySet(set);

            Assert.AreEqual(2, cards.Count());
        }

        [Test]
        public void GetCardsBySet_SetDoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "Set";

            Exception ex = Assert.Throws<Exception>(
            delegate
            {
                var cards = mtgStore.GetCardsBySet(set);
            });

            Assert.That(ex.Message, Is.EqualTo(string.Format("No set exists using name '{0}'.", set.ToLower())));
        }
        #endregion

        #region SetExists
        [Test]
        public void SetExists_True()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "Gatecrash";

            bool exists = mtgStore.SetExists(set);

            Assert.True(exists);
        }

        [Test]
        public void SetExists_False()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "SET";

            bool exists = mtgStore.SetExists(set);

            Assert.False(exists);
        }
        #endregion

        #region SetExistsByCode
        [Test]
        public void SetExistsByCode_True()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "GTC";

            bool exists = mtgStore.SetExistsByCode(set);

            Assert.True(exists);
        }

        [Test]
        public void SetExistsByCode_False()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "CODE";

            bool exists = mtgStore.SetExistsByCode(set);

            Assert.False(exists);
        }
        #endregion

        #region GetSet
        [Test]
        public void GetSet()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "Gatecrash";

            Set actual = mtgStore.GetSet(set);

            Assert.NotNull(actual);
            Assert.AreEqual("Gatecrash", actual.Name);
        }

        [Test]
        public void GetSet_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "SETNAME";

            Set actual = mtgStore.GetSet(set);

            Assert.Null(actual);
        }
        #endregion

        #region GetSetByCode
        [Test]
        public void GetSetByCode()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "GTC";

            Set actual = mtgStore.GetSetByCode(set);

            Assert.NotNull(actual);
            Assert.AreEqual("Gatecrash", actual.Name);
        }

        [Test]
        public void GetSetByCode_DoesntExist()
        {
            var mtgStore = new MtgStore(contextMock.Object);

            string set = "XXX";

            Set actual = mtgStore.GetSetByCode(set);

            Assert.Null(actual);
        }
        #endregion
    }
}
