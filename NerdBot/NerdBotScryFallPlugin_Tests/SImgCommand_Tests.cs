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
    public class SImgCommand_Tests
    {
        private TestConfiguration testConfig;

        private ScryFallImgPlugin simgCommandPlugin;
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

            simgCommandPlugin = new ScryFallImgPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            simgCommandPlugin.LoggingService = unitTestContext.LoggingServiceMock.Object;
        }

        [Test]
        public void SImgCommand_NoArguments()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByName()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);
            
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/png/en/exp/43.png?1509690533"));
        }

        [Test]
        public void SImgCommand_ByName_OtherSets()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            unitTestContext.StoreMock.Setup(s => s.GetCardOtherSets(testData.TestCard.MultiverseId))
                .ReturnsAsync(new List<Set>()
                {
                    new Set()
                    {
                        Name = "Unstable",
                        Code = "UST"
                    }
                });

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/png/en/exp/43.png?1509690533"));
            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Also in sets: UST"))));
        }

        [Test]
        public void SImgCommand_ByNameAndSet()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardPng_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/png/en/exp/43.png?1509690533"));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoCardFound()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard_ScryNotFound.MultiverseId))
                .ReturnsAsync("");

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void SImgCommand_ByName_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    ""
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    ""
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoSet()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_LargeImg()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardLarge_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/large/en/exp/43.jpg?1509690533"));
        }

        [Test]
        public void SImgCommand_NormalImg()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardNormal_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/normal/en/exp/43.jpg?1509690533"));
        }

        [Test]
        public void SImgCommand_SmallImg()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardSmall_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"));
        }

        [Test]
        public void SImgCommand_NoImg()
        {
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsJson("https://api.scryfall.com/cards/multiverse/" + testData.TestCard.MultiverseId))
                .ReturnsAsync(testData.TestCardNoImage_Json);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(testData.TestCard.Img));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound_RunAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoCardFound_RunAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound_NoAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }

        [Test]
        public void SImgCommand_ByNameSet_NoCardFound_NoAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }
    }
}