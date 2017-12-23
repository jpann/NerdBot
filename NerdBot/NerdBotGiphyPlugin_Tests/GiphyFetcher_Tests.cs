using System.Threading.Tasks;
using NerdBot.TestsHelper;
using NerdBotGiphyPlugin;
using NUnit.Framework;

namespace NerdBotGiphyPlugin_Tests
{
    [TestFixture]
    public class GiphyFetcher_Tests
    {
        private GiphyFetcher fetcher;
        private UnitTestContext unitTestContext;

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
            unitTestContext = new UnitTestContext();
        }

        [SetUp]
        public void SetUp()
        {
            string url = "http://localhost/{0}";

            fetcher = new GiphyFetcher(url, unitTestContext.HttpClientMock.Object);
        }

        [Test]
        public void GetGiphyGif()
        {
            string keyword = "cat";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(giphydata);

            unitTestContext.HttpClientMock.Setup(h => h.GetAsJsonNonCached(string.Format(url, keyword)))
                .Returns(httpJsonTask.Task);

            string actual = fetcher.GetGiphyGif(keyword).Result;

            Assert.AreEqual(giphyUrl, actual);
        }
    }
}