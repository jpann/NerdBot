using System.Threading.Tasks;
using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotCoreCommands;
using NUnit.Framework;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    public class RandomFlavorTextPlugin_Tests
    {
        private RandomFlavorTextPlugin plugin;

        private UnitTestContext unitTestContext;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {

        }
            
        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            plugin = new RandomFlavorTextPlugin(
                unitTestContext.BotServicesMock.Object,
                unitTestContext.BotConfig);

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;

            plugin.OnLoad();
        }

        [Test]
        public void GetFlavor()
        {
            unitTestContext.StoreMock.Setup(s => s.GetRandomFlavorText())
                .Returns(() => Task.FromResult("Flavor text."));

            var cmd = new Command()
            {
                Cmd = "flavor",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetFlavor_NoText()
        {
            unitTestContext.StoreMock.Setup(s => s.GetRandomFlavorText())
                .Returns(() => Task.FromResult(""));

            var cmd = new Command()
            {
                Cmd = "flavor",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
        }
    }
}
