using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBot.Http;
using NerdBot.Messengers.GroupMe;
using NUnit.Framework;

namespace NerdBot.Tests
{
    [TestFixture]
    class GroupMeMessenger_Tests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void SendMessage()
        {
            string json = @"";

            var httpClientMock = new Mock<IHttpClient>();

        }
    }
}
