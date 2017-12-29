using System.Collections.Generic;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotScryFallPlugin;
using NUnit.Framework;

namespace NerdBotScryFallPlugin_Tests
{
    public class ScryCommand_Tests
    {
        private TestConfiguration testConfig;

        private ScryFallPricePlugin scryCommandPlugin;
        private UnitTestContext unitTestContext;
        private TestData testData;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            testConfig = new ConfigReader().Read();

            testData = new TestData();
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            unitTestContext.StoreMock.Setup(s => s.GetCardOtherSets(It.IsAny<int>()))
                .ReturnsAsync(() => new List<Set>());

            unitTestContext.StoreMock.Setup(s => s.GetCard(testData.TestCard.Name))
                .ReturnsAsync(() => testData.TestCard);

            unitTestContext.StoreMock.Setup(s => s.GetCard(testData.TestCard.Name, testData.TestCard.SetId))
                .ReturnsAsync(() => testData.TestCard);

            unitTestContext.StoreMock.Setup(s => s.GetCard(testData.TestCard_ScryNotFound.Name))
                .ReturnsAsync(() => testData.TestCard_ScryNotFound);

            unitTestContext.StoreMock.Setup(s => s.GetCard(testData.TestCard_ScryNotFound.Name, testData.TestCard_ScryNotFound.SetId))
                .ReturnsAsync(() => testData.TestCard_ScryNotFound);

            unitTestContext.StoreMock.Setup(s => s.GetCard("not found"))
                .ReturnsAsync(() => null);

            scryCommandPlugin = new ScryFallPricePlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            scryCommandPlugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void ScryCommand_NoArguments()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByName()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Strip Mine [EXP] - $53.54"))));
        }

        [Test]
        public void ScryCommand_ByNameAndSet()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Strip Mine [EXP] - $53.54"))));
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoCardFound()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void ScryCommand_ByName_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    ""
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    ""
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoSet()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_NoPrice()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardNoUSDPrice_Json);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound_RunAutocomplete()
        {
            string name = "spore bloud";

            // Setup IAutocompleter mock response
            unitTestContext.AutocompleterMock.Setup(ac => ac.GetAutocompleteAsync("spore"))
                .ReturnsAsync(() => new List<string>()
                {
                    "Spore Frog",
                    "Spore Burst",
                    "Sporemound",
                    "Spore Cloud",
                    "Spore Flower"
                });

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoCardFound_RunAutocomplete()
        {
            string name = "spore bloud";

            // Setup IAutocompleter mock response
            unitTestContext.AutocompleterMock.Setup(ac => ac.GetAutocompleteAsync("spore"))
                .ReturnsAsync(() => new List<string>()
                {
                    "Spore Frog",
                    "Spore Burst",
                    "Sporemound",
                    "Spore Cloud",
                    "Spore Flower"
                });

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound_NoAutocomplete()
        {
            string name = "spore bloud";

            // Setup IAutocompleter mock response
            unitTestContext.AutocompleterMock.Setup(ac => ac.GetAutocompleteAsync("spore"))
                .ReturnsAsync(() => new List<string>()
                {
                });

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }

        [Test]
        public void ScryCommand_ByNameSet_NoCardFound_NoAutocomplete()
        {
            string name = "spore bloud";

            // Setup IAutocompleter mock response
            unitTestContext.AutocompleterMock.Setup(ac => ac.GetAutocompleteAsync("spore"))
                .ReturnsAsync(() => new List<string>()
                {
                });

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "C13",
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }
    }
}