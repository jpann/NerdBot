using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBotCoreCommands;
using NUnit.Framework;

namespace NerdBotCoreCommandsPlugin_Tests
{
    [TestFixture]
    class RandomSetCardPlugin_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private RandomSetCardPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            mtgStore = new MtgStore(connectionString, databaseName);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICommandParser Mocks
            commandParserMock = new Mock<ICommandParser>();

            // Setup IHttpClient Mocks
            httpClientMock = new Mock<IHttpClient>();

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new RandomSetCardPlugin(
                mtgStore,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);
        }

        [Test]
        public void GetRandomSetCard()
        {
            var cmd = new Command()
            {
                Cmd = "setrandom",
                Arguments = new string[]
                {
                    "fem"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Once);
        }
    }
}
