using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers.GroupMe;
using Ninject.Extensions.Logging;
using NUnit.Framework;

namespace NerdBot.Tests
{
    [TestFixture]
    class GroupMeMessenger_Tests
    {
        [Test]
        public void SendMessage()
        {
            string url = "https://api.groupme.com/v3/bots/post";
            string expectedJson = @"{""text"":""Message here"",""bot_id"":""BOT_ID""}";
            
            var httpClientMock = new Mock<IHttpClient>();
            var loggerMock = new Mock<ILogger>();

            var messenger = new GroupMeMessenger("BOT_ID", "BOT_NAME", new string[] {}, url, httpClientMock.Object, loggerMock.Object);

            messenger.SendMessage("Message here");

            httpClientMock.Verify(c => c.Post(url, expectedJson));
        }

        [Test]
        public void SendMessage_Failure()
        {
            string url = "https://api.groupme.com/v3/bots/post";
            string expectedJson = @"{""text"":""Message here"",""bot_id"":""BOT_ID""}";

            var httpClientMock = new Mock<IHttpClient>();
            var loggerMock = new Mock<ILogger>();

            // Mock failure
            httpClientMock.Setup(c => c.Post(url, expectedJson))
                .Throws(new System.Net.WebException("ERROR"));

            // Mock error logging
            loggerMock.Setup(c => c.Error(It.IsAny<Exception>(), "Error sending groupme message: ERROR"));

            var messenger = new GroupMeMessenger("BOT_ID", "BOT_NAME", new string[] { }, url, httpClientMock.Object, loggerMock.Object);

            bool actualResult = messenger.SendMessage("Message here");

            Assert.False(actualResult);
        }
    }
}
