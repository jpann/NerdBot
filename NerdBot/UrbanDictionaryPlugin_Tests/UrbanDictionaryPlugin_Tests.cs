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
using NerdBotUrbanDictionary;
using NUnit.Framework;
using SimpleLogging.Core;

namespace UrbanDictionaryPlugin_Tests
{
    [TestFixture]
    public class UrbanDictionaryPlugin_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private UrbanDictionaryPlugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private IHttpClient httpClient;
        private Mock<IUrlShortener> urlShortenerMock;
        private Mock<IMessenger> messengerMock;
        private Mock<ILoggingService> loggingServiceMock;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggingServiceMock = new Mock<ILoggingService>();

            mtgStore = new MtgStore(connectionString, databaseName, loggingServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // Setup ICommandParser Mocks
            commandParserMock = new Mock<ICommandParser>();

            httpClient = new SimpleHttpClient(loggingServiceMock.Object);

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new UrbanDictionaryPlugin(
                mtgStore,
                commandParserMock.Object,
                httpClient,
                urlShortenerMock.Object);
        }

        [Test]
        public void GetDefinition_IsA()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is a butt"   
                }
            };

            var msg = new GroupMeMessage();

            string expected = "1. Verb - to press up against or to jostle.\r\n2. Noun - the end part of a rifle or shotgun or machinegun that rests against the shoulder or pectoral muscles to increase stability during firing.\r\n3. Noun - the part of a human being that knows wind and earth. The buttocks and anus of a person.\r\n4. Noun - the recipient or target of a joke.";
            
            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(expected), Times.Once);
        }

        [Test]
        public void GetDefinition_Is()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is butt"   
                }
            };

            var msg = new GroupMeMessage();

            string expected = "1. Verb - to press up against or to jostle.\r\n2. Noun - the end part of a rifle or shotgun or machinegun that rests against the shoulder or pectoral muscles to increase stability during firing.\r\n3. Noun - the part of a human being that knows wind and earth. The buttocks and anus of a person.\r\n4. Noun - the recipient or target of a joke.";

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                messengerMock.Object
                ).Result;

            messengerMock.Verify(m => m.SendMessage(expected), Times.Once);
        }
    }
}
