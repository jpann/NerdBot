using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBotCommon.Tests
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
            var loggerMock = new Mock<ILoggingService>();

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
            var loggerMock = new Mock<ILoggingService>();

            // Mock failure
            httpClientMock.Setup(c => c.Post(url, expectedJson))
                .Throws(new System.Net.WebException("ERROR"));

            // Mock error logging
            loggerMock.Setup(c => c.Error(It.IsAny<Exception>(), "Error sending groupme message: ERROR", true));

            var messenger = new GroupMeMessenger("BOT_ID", "BOT_NAME", new string[] { }, url, httpClientMock.Object, loggerMock.Object);

            bool actualResult = messenger.SendMessage("Message here");

            Assert.False(actualResult);
        }
    }
}
