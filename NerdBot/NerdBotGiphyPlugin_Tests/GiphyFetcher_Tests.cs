using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NerdBot.Parsers;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using NerdBotGiphyPlugin;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotGiphyPlugin_Tests
{
    [TestFixture]
    public class GiphyFetcher_Tests
    {
        private GiphyFetcher fetcher;
        private Mock<IHttpClient> httpClientMock;
        private Mock<ILoggingService> loggingServiceMock;

        private string url = "http://localhost/{0}";
        private string giphyUrl = @"https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.gif";

        private string giphydata = @"{
  ""data"": {
    ""images"": {
      ""original"": {
        ""url"": ""https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.gif"",
        ""width"": ""400"",
        ""height"": ""250"",
        ""size"": ""2535859"",
        ""frames"": ""111"",
        ""mp4"": ""https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.mp4"",
        ""mp4_size"": ""472711"",
        ""webp"": ""https///media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.webp"",
        ""webp_size"": ""3193862""
      }
    }
  }
}";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            httpClientMock = new Mock<IHttpClient>();
        }

        [SetUp]
        public void SetUp()
        {
            string url = "http://localhost/{0}";

            fetcher = new GiphyFetcher(url, httpClientMock.Object);
        }

        [Test]
        public void GetGiphyGif()
        {
            string keyword = "cat";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(giphydata);

            httpClientMock.Setup(h => h.GetAsJson(string.Format(url, keyword)))
                .Returns(httpJsonTask.Task);

            string actual = fetcher.GetGiphyGif(keyword).Result;

            Assert.AreEqual(giphyUrl, actual);
        }
    }
}