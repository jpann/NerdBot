using Moq;
using NerdBot.TestsHelper;
using NerdBotCommon.Http;
using NerdBotWhatsInStandardPlugin;
using NUnit.Framework;

namespace NerdBotWhatsInStandardPlugin_Tests
{
    public class WhatsInStandardFetcher_Tests
    {
        private WhatsInStandardFetcher fetcher;
        private IHttpClient httpClient;
        private UnitTestContext unitTestContext;

        private string whatsInStandardData;

        #region Setup Data

        private void SetupData()
        {
            whatsInStandardData = @"{
deprecated: false,
sets: [
{
name: ""Battle for Zendikar"",
block: ""Battle for Zendikar"",
code: ""BFZ"",
enter_date: ""2015-10-02T00:00:00.000Z"",
exit_date: ""2017-09-29T00:00:00.000Z"",
rough_exit_date: ""Q4 2017""
},
{
name: ""Oath of the Gatewatch"",
block: ""Battle for Zendikar"",
code: ""OGW"",
enter_date: ""2016-01-22T00:00:00.000Z"",
exit_date: ""2017-09-29T00:00:00.000Z"",
rough_exit_date: ""Q4 2017""
},
{
name: ""Shadows over Innistrad"",
block: ""Shadows over Innistrad"",
code: ""SOI"",
enter_date: ""2016-04-08T00:00:00.000Z"",
exit_date: ""2017-09-29T00:00:00.000Z"",
rough_exit_date: ""Q4 2017""
},
{
name: ""Welcome Deck 2016"",
block: ""Shadows over Innistrad"",
code: ""W16"",
enter_date: ""2016-04-09T00:00:00.000Z"",
exit_date: ""2017-09-29T00:00:00.000Z"",
rough_exit_date: ""Q4 2017""
},
{
name: ""Eldritch Moon"",
block: ""Shadows over Innistrad"",
code: ""EMN"",
enter_date: ""2016-07-21T00:00:00.000Z"",
exit_date: ""2017-09-29T00:00:00.000Z"",
rough_exit_date: ""Q4 2017""
},
{
name: ""Kaladesh"",
block: ""Kaladesh"",
code: ""KLD"",
enter_date: ""2016-09-30T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2018""
},
{
name: ""Aether Revolt"",
block: ""Kaladesh"",
code: ""AER"",
enter_date: ""2017-01-20T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2018""
},
{
name: ""Amonkhet"",
block: ""Amonkhet"",
code: ""AKH"",
enter_date: ""2017-04-28T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2018""
},
{
name: ""Welcome Deck 2017"",
block: ""Amonkhet"",
code: ""W17"",
enter_date: ""2017-04-28T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2018""
},
{
name: ""Hour of Devastation"",
block: ""Amonkhet"",
code: ""HOU"",
enter_date: ""2017-07-14T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2018""
},
{
name: ""Ixalan"",
block: ""Ixalan"",
code: ""XLN"",
enter_date: ""2017-09-29T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2019""
},
{
name: ""Rivals of Ixalan"",
block: ""Ixalan"",
code: ""RIX"",
enter_date: ""2018-01-19T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2019""
},
{
name: ""Dominaria"",
block: null,
code: ""DOM"",
enter_date: ""2018-04-27T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2019""
},
{
name: ""Core 2019"",
block: null,
code: ""M19"",
enter_date: ""2018-07-20T00:00:00.000Z"",
exit_date: null,
rough_exit_date: ""Q4 2019""
}
]
}";
        }

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SetupData();
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            fetcher = new WhatsInStandardFetcher(unitTestContext.HttpClientMock.Object);
        }

        [Test]
        public void GetSetsInStandard_Filtered()
        {
            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://whatsinstandard.com/api/v5/sets.json"))
                .ReturnsAsync(whatsInStandardData);

            int expectedCount = 6;

            var actual = fetcher.GetDataAsync(true).Result;

            Assert.AreEqual(expectedCount, actual.Sets.Count);
        }

        [Test]
        public void GetSetsInStandard_NotFiltered()
        {
            unitTestContext.HttpClientMock.Setup(h => h.GetAsJson("http://whatsinstandard.com/api/v5/sets.json"))
                .ReturnsAsync(whatsInStandardData);

            int expectedCount = 14;

            var actual = fetcher.GetDataAsync(false).Result;

            Assert.AreEqual(expectedCount, actual.Sets.Count);
        }
    }
}