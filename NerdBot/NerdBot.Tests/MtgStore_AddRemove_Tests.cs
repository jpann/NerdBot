using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using Moq;
using NerdBot.Mtg;
using NerdBot.Utilities;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests
{
    [TestFixture]
    public class MtgStore_AddRemove_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb_ins_testing";

        private IMtgStore mtgStore;
        private Mock<ILoggingService> loggingServiceMock;
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

        public void RemoveTestData()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);
            var cardCollection = database.GetCollection<Card>("cards");
            var setCollection = database.GetCollection<Set>("sets");

            // Remove everything
            cardCollection.RemoveAll();
            setCollection.RemoveAll();
        }

        #region Test Data
        private Card testCard = new Card()
        {
            RelatedCardId = 1,
            SetNumber = 1,
            Name = "Abyssal Persecutor",
            SearchName = "abyssalpersecutor",
            Desc = "Flying, trample\nYou can't win the game and your opponents can't lose the game.",
            Flavor = "His slaves crave death more than they desire freedom. He denies them both.",
            Colors = new string[] { "Black" },
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
        #endregion

        [SetUp]
        public void SetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();
            searchUtilityMock = new Mock<SearchUtility>();

            searchUtilityMock.Setup(s => s.GetSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetSearchValue(s));

            searchUtilityMock.Setup(s => s.GetRegexSearchValue(It.IsAny<string>()))
                .Returns((string s) => this.GetRegexSearchValue(s));

            mtgStore = new MtgStore(
                connectionString, 
                databaseName, 
                loggingServiceMock.Object, 
                searchUtilityMock.Object);
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
        #endregion

        #region AddSet
        [Test]
        public void AddSet()
        {

        }
        #endregion
    }
}
