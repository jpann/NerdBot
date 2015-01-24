using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
using NerdBotTappedOut;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotTappedOutPlugin_Tests
{
    [TestFixture]
    class TappedOutLatestTop8Plugin_Tests
    {
        private const string connectionString = "mongodb://localhost";
        private const string databaseName = "mtgdb";

        private IMtgStore mtgStore;
        private TappedOutLatestTop8Plugin plugin;

        private Mock<ICommandParser> commandParserMock;
        private Mock<IHttpClient> httpClientMock;
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

            // Setup IHttpClient Mocks
            httpClientMock = new Mock<IHttpClient>();

            // Setup IUrlShortener Mocks
            urlShortenerMock = new Mock<IUrlShortener>();

            // Setup IMessenger Mocks
            messengerMock = new Mock<IMessenger>();

            plugin = new TappedOutLatestTop8Plugin(
                mtgStore,
                commandParserMock.Object,
                httpClientMock.Object,
                urlShortenerMock.Object);
        }  
    }
}
