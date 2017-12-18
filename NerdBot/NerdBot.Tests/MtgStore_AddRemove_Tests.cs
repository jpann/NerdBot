using System;
using System.Collections.Generic;
using MongoDB.Driver;
using NerdBot.TestsHelper;
using NerdBotCommon.Mtg;
using NUnit.Framework;

namespace NerdBot.Tests
{
    [TestFixture]
    public class MtgStore_AddRemove_Tests
    {
        private TestConfiguration testConfig;

        private IMtgStore mtgStore;
        private UnitTestContext unitTestContext;

        // Test data
        private Card testCard;
        private Card testCardExists;
        private Card testCardDoesntExist;
        private Card testCardToRemove;
        private Set testSet;
        private Set testSetExists;
        private Set testSetDoesntExist;
        private Set testSetToRemove;

        #region Test Data Methods
        public void InsertTestData()
        {
            var client = new MongoClient(testConfig.Url);
            var database = client.GetDatabase(testConfig.TestDb);
            var cardCollection = database.GetCollection<Card>("cards");
            var setCollection = database.GetCollection<Set>("sets");

            cardCollection.InsertOne(testCardExists);
            cardCollection.InsertOne(testCardToRemove);

            setCollection.InsertOne(testSetExists);
            setCollection.InsertOne(testSetToRemove);
        }

        public void RemoveTestData()
        {
            var client = new MongoClient(testConfig.Url);
            var server = client.GetServer();
            var database = server.GetDatabase(testConfig.TestDb);
            var cardCollection = database.GetCollection<Card>("cards");
            var setCollection = database.GetCollection<Set>("sets");

            // Remove everything
            cardCollection.RemoveAll();
            setCollection.RemoveAll();
        }

        public void CreateTestData()
        {
            testCard = new Card()
            {
                RelatedCardId = 1,
                Name = "Abyssal Persecutor",
                SearchName = "abyssalpersecutor",
                Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
                Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
                Colors = new List<string>() { "Black" },
                Cost = "{2}{B}{B}",
                Cmc = 4,
                SetName = "Commander 2014",
                SetSearchName = "commander2014",
                Type = "Creature",
                SubType = "Demon",
                Power = "6",
                Toughness = "6",
                Loyalty = "",
                Artist = "Chippy",
                SetId = "C14",
                Token = false,
                Img = "http://mtgimage.com/multiverseid/389422.jpg",
                ImgHires = "http://mtgimage.com/multiverseid/389422.jpg",
                MultiverseId = 389422,
                Rarity = "Mythic Rare",
                Rulings = new List<Ruling>()
                    {
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "No game effect can cause you to win the game or cause any opponent to lose the game while you control Abyssal Persecutor. It doesn't matter whether an opponent has 0 or less life, an opponent is forced to draw a card while his or her library is empty, an opponent has ten or more poison counters, an opponent is dealt combat damage by Phage the Untouchable, you control Felidar Sovereign and have 40 or more life, or so on. You keep playing.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Other circumstances can still cause an opponent to lose the game, however. An opponent will lose a game if he or she concedes, if that player is penalized with a Game Loss or a Match Loss during a sanctioned tournament due to a DCI rules infraction, or if that player's _Magic Online_(R) game clock runs out of time.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Effects that say the game is a draw, such as the _Legends_(TM) card Divine Intervention, are not affected by Abyssal Persecutor.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Abyssal Persecutor won't preclude an opponent's life total from reaching 0 or less. It will just preclude that player from losing the game as a result.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "If Abyssal Persecutor leaves the battlefield while an opponent has 0 or less life, that opponent will lose the game as a state-based action. No player can respond between the time Abyssal Persecutor leaves the battlefield and the time that player loses the game.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Even though your opponents can't lose the game, a player can't pay an amount of life that's greater than his or her life total. If a player's life total is 0 or less, that player can't pay life at all, with one exception: a player may always pay 0 life.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "If you control Abyssal Persecutor in a Two-Headed Giant game, your team can't win the game and the opposing team can't lose the game.",
                        }
                    }
            };

            testCardToRemove = new Card()
            {
                RelatedCardId = 1,
                Name = "Abyssal Persecutor",
                SearchName = "abyssalpersecutor",
                Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
                Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
                Colors = new List<string>() { "Black" },
                Cost = "{2}{B}{B}",
                Cmc = 4,
                SetName = "Commander 2014",
                SetSearchName = "commander2014",
                Type = "Creature",
                SubType = "Demon",
                Power = "6",
                Toughness = "6",
                Loyalty = "",
                Artist = "Chippy",
                SetId = "C14",
                Token = false,
                Img = "http://mtgimage.com/multiverseid/389422.jpg",
                ImgHires = "http://mtgimage.com/multiverseid/389422.jpg",
                MultiverseId = 99999,
                Rarity = "Mythic Rare",
                Rulings = new List<Ruling>()
                {
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "No game effect can cause you to win the game or cause any opponent to lose the game while you control Abyssal Persecutor. It doesn't matter whether an opponent has 0 or less life, an opponent is forced to draw a card while his or her library is empty, an opponent has ten or more poison counters, an opponent is dealt combat damage by Phage the Untouchable, you control Felidar Sovereign and have 40 or more life, or so on. You keep playing.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "Other circumstances can still cause an opponent to lose the game, however. An opponent will lose a game if he or she concedes, if that player is penalized with a Game Loss or a Match Loss during a sanctioned tournament due to a DCI rules infraction, or if that player's _Magic Online_(R) game clock runs out of time.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "Effects that say the game is a draw, such as the _Legends_(TM) card Divine Intervention, are not affected by Abyssal Persecutor.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "Abyssal Persecutor won't preclude an opponent's life total from reaching 0 or less. It will just preclude that player from losing the game as a result.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "If Abyssal Persecutor leaves the battlefield while an opponent has 0 or less life, that opponent will lose the game as a state-based action. No player can respond between the time Abyssal Persecutor leaves the battlefield and the time that player loses the game.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "Even though your opponents can't lose the game, a player can't pay an amount of life that's greater than his or her life total. If a player's life total is 0 or less, that player can't pay life at all, with one exception: a player may always pay 0 life.",
                    },
                    new Ruling()
                    {
                        ReleasedOn = new DateTime(2010,3,1),
                        Rule = "If you control Abyssal Persecutor in a Two-Headed Giant game, your team can't win the game and the opposing team can't lose the game.",
                    }
                }
            };

            testCardExists = new Card()
            {
                RelatedCardId = 1,
                Name = "Abyssal Exists",
                SearchName = "abyssalExists",
                Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
                Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
                Colors = new List<string>() { "Black" },
                Cost = "{2}{B}{B}",
                Cmc = 4,
                SetName = "Commander 2014",
                SetSearchName = "commander2014",
                Type = "Creature",
                SubType = "Demon",
                Power = "6",
                Toughness = "6",
                Loyalty = "",
                Artist = "Chippy",
                SetId = "C14",
                Token = false,
                Img = "http://mtgimage.com/multiverseid/389422.jpg",
                ImgHires = "http://mtgimage.com/multiverseid/389422.jpg",
                MultiverseId = 389422,
                Rarity = "Mythic Rare",
                Rulings = new List<Ruling>()
                    {
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "No game effect can cause you to win the game or cause any opponent to lose the game while you control Abyssal Persecutor. It doesn't matter whether an opponent has 0 or less life, an opponent is forced to draw a card while his or her library is empty, an opponent has ten or more poison counters, an opponent is dealt combat damage by Phage the Untouchable, you control Felidar Sovereign and have 40 or more life, or so on. You keep playing.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Other circumstances can still cause an opponent to lose the game, however. An opponent will lose a game if he or she concedes, if that player is penalized with a Game Loss or a Match Loss during a sanctioned tournament due to a DCI rules infraction, or if that player's _Magic Online_(R) game clock runs out of time.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Effects that say the game is a draw, such as the _Legends_(TM) card Divine Intervention, are not affected by Abyssal Persecutor.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Abyssal Persecutor won't preclude an opponent's life total from reaching 0 or less. It will just preclude that player from losing the game as a result.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "If Abyssal Persecutor leaves the battlefield while an opponent has 0 or less life, that opponent will lose the game as a state-based action. No player can respond between the time Abyssal Persecutor leaves the battlefield and the time that player loses the game.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "Even though your opponents can't lose the game, a player can't pay an amount of life that's greater than his or her life total. If a player's life total is 0 or less, that player can't pay life at all, with one exception: a player may always pay 0 life.",
                        },
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "If you control Abyssal Persecutor in a Two-Headed Giant game, your team can't win the game and the opposing team can't lose the game.",
                        }
                    }
            };

            testCardDoesntExist = new Card()
            {
                RelatedCardId = 1,
                Name = "Abyssal",
                SearchName = "abyssal",
                Desc = "Flying, trample",
                Flavor = "His slaves",
                Colors = new List<string>() { "Black" },
                Cost = "{2}{B}{B}",
                Cmc = 4,
                SetName = "Commander 2014",
                SetSearchName = "commander2014",
                Type = "Creature",
                SubType = "Demon",
                Power = "6",
                Toughness = "6",
                Loyalty = "",
                Artist = "Chippy",
                SetId = "C14",
                Token = false,
                Img = "http://mtgimage.com/multiverseid/389422.jpg",
                ImgHires = "http://mtgimage.com/multiverseid/389422.jpg",
                MultiverseId = 100000,
                Rarity = "Mythic Rare",
                Rulings = new List<Ruling>()
                    {
                        new Ruling()
                        {
                            ReleasedOn = new DateTime(2010,3,1),
                            Rule = "No game effect can cause you to win the game or cause any opponent to lose the game while you control Abyssal Persecutor. It doesn't matter whether an opponent has 0 or less life, an opponent is forced to draw a card while his or her library is empty, an opponent has ten or more poison counters, an opponent is dealt combat damage by Phage the Untouchable, you control Felidar Sovereign and have 40 or more life, or so on. You keep playing.",
                        }
                    }
            };

            testSet = new Set()
            {
                Name = "Commander 2014",
                Code = "C14",
                SearchName = "commander2014",
                Type = "Commander",
                ReleasedOn = new DateTime(2014, 11, 7)
            };

            testSetToRemove = new Set()
            {
                Name = "Commander 2019",
                Code = "C19",
                SearchName = "commander2019",
                Type = "Commander",
                ReleasedOn = new DateTime(2019, 11, 7)
            };

            testSetExists = new Set()
            {
                Name = "Commander 2015",
                SearchName = "commander2015",
                Code = "C15",
                Type = "Commander",
                ReleasedOn = new DateTime(2015, 11, 7)
            };

            testSetDoesntExist = new Set()
            {
                Name = "Commander DoesntExist",
                SearchName = "commanderdoesntexist",
                Code = "CXX",
                Type = "Commander",
                ReleasedOn = new DateTime(2015, 11, 7)
            };
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            testConfig = new ConfigReader().Read();
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            mtgStore = new MtgStore(
                testConfig.Url,
                testConfig.TestDb,
                unitTestContext.QueryStatisticsStoreMock.Object,
                unitTestContext.LoggingServiceMock.Object,
                unitTestContext.SearchUtilityMock.Object);

            CreateTestData();
            InsertTestData();
        }

        [TearDown]
        public void TearDown()
        {
            RemoveTestData();
        }

        #region AddCard
        [Test]
        public void AddCard()
        {
            var actual = mtgStore.AddCard(testCard).Result;

            Assert.NotNull(actual);
        }

        [Test]
        public void CardFindAndModify_Exists()
        {
            // Modify the colors of the card
            testCardExists.Colors = new List<string>() { "Black", "White" };

            var actual = mtgStore.CardFindAndModify(testCardExists).Result;

            Assert.NotNull(actual);
            Assert.True(actual.Colors.Contains("White"));
        }

        [Test]
        public void CardFindAndModify_DoesntExist()
        {
            var actual = mtgStore.CardFindAndModify(testCardDoesntExist).Result;

            Assert.NotNull(actual);
            Assert.True(actual.Colors.Contains("Black"));
        }

        [Test]
        public void RemoveCard()
        {
            int expected = 1;
            int actual = mtgStore.RemoveCard(testCardToRemove).Result;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveCard_DoesntExist()
        {
            int expected = 0;
            int actual = mtgStore.RemoveCard(testCardDoesntExist).Result;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveCard_ByMultiverseId()
        {
            int expected = 1;
            int actual = mtgStore.RemoveCard(testCardToRemove.MultiverseId).Result;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RemoveCard_ByMultiverseId_DoesntExist()
        {
            int expected = 0;
            int actual = mtgStore.RemoveCard(testCardDoesntExist.MultiverseId).Result;

            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region AddSet
        [Test]
        public void AddSet()
        {
            var actual = mtgStore.AddSet(testSet).Result;

            Assert.NotNull(actual);
        }

        [Test]
        public void SetFindAndModify_Exists()
        {
            testSetExists.Desc = "TEST";

            var actual = mtgStore.SetFindAndModify(testSetExists).Result;

            Assert.NotNull(actual);
            Assert.AreEqual("TEST", actual.Desc);
        }

        [Test]
        public void SetFindAndModify_DoesntExists()
        {
            var actual = mtgStore.SetFindAndModify(testSetDoesntExist).Result;

            Assert.NotNull(actual);
        }

        [Test]
        public void RemoveSet()
        {
            int expected = 1;

            var actual = mtgStore.RemoveSet(testSetToRemove).Result;

            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
