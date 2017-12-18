using Moq;
using NerdBot;
using NerdBot.TestsHelper;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotCoreCommands;
using NUnit.Framework;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    public class CoinFlipPlugin_Tests
    {
        private CoinFlipPlugin plugin;

        private UnitTestContext unitTestContext;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {

        }
            
        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();
            
            plugin = new CoinFlipPlugin(
                unitTestContext.BotServicesMock.Object,
                new BotConfig());

            plugin.LoggingService = unitTestContext.LoggingServiceMock.Object;

            plugin.OnLoad();
        }

        [Test]
        public void PerformFlip()
        {
            var cmd = new Command()
            {
                Cmd = "coinflip",
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

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Coin flip"))), Times.Once);
        }
    }
}
