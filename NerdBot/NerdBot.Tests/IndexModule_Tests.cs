//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Moq;
//using Nancy;
//using Nancy.Testing;
//using NerdBot.Http;
//using NerdBot.Messengers;
//using NerdBot.Messengers.GroupMe;
//using NerdBot.Mtg;
//using Ninject.Extensions.Logging;
//using NUnit.Framework;

//namespace NerdBot.Tests
//{
//    [TestFixture]
//    class IndexModule_Tests
//    {
//        private Browser browser;

//        private Mock<DbSet<Card>> cardDbSetMock;
//        private Mock<DbSet<Set>> setDbSetMock;
//        private Mock<IMtgContext> contextMock;
//        private Mock<IHttpClient> httpClientMock;
//        private Mock<ILogger> loggerMock;
//        private Mock<IPluginManager> pluginManagerMock;
//        private Mock<IMessenger> messengerMock;

//        private List<Card> cardData = new List<Card>();
//        private List<Set> setData = new List<Set>();

//        private void SetUp_Data()
//        {
//            cardData = new List<Card>()
//            {
//                new Card()
//                {
//                    Id = 5891,
//                    Related_Card_Id = 0,
//                    Set_Number =  179,
//                    Name = "Boros Charm",
//                    Search_Name = "Boros Charm",
//                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control gain indestructible until end of turn; or target creature gains double strike until end of turn.",
//                    Flavor = "",
//                    Colors = "red;white",
//                    Cost = "RW",
//                    Cmc = 2,
//                    Set_Name = "Commander 2013 Edition",
//                    Type = "Instant",
//                    SubType = "",
//                    Power = "0",
//                    Toughness = "00",
//                    Loyalty = "0",
//                    Rarity = "Uncommon",
//                    Artist = "Zoltan Boros",
//                    Set_Id = "C13",
//                    Token = false,
//                    Img = "https://api.mtgdb.info/content/card_images/376270.jpeg",
//                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/376270.jpg",
//                    Multiverse_Id = 376270,
//                },
//                new Card()
//                {
//                    Id = 6975,
//                    Related_Card_Id = 0,
//                    Set_Number =  137,
//                    Name = "Boros Cluestone",
//                    Search_Name = "Boros Cluestone",
//                    Desc = "{Tap}: Add {Red} or {White} to your mana pool.\n\r" +
//                            "{Red}{White}, {Tap}, Sacrifice Boros Cluestone: Draw a card.",
//                    Flavor = "",
//                    Colors = "None",
//                    Cost = "3",
//                    Cmc = 3,
//                    Set_Name = "Dragon's Maze",
//                    Type = "Artifact",
//                    SubType = "",
//                    Power = "0",
//                    Toughness = "0",
//                    Loyalty = "0",
//                    Rarity = "Common",
//                    Artist = "Raoul Vitale",
//                    Set_Id = "DGM",
//                    Token = false,
//                    Img = "https://api.mtgdb.info/content/card_images/368997.jpeg",
//                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/368997.jpg",
//                    Multiverse_Id = 368997,
//                },
//                new Card()
//                {
//                    Id = 7213,
//                    Related_Card_Id = 0,
//                    Set_Number =  148,
//                    Name = "Boros Charm",
//                    Search_Name = "Boros Charm",
//                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control are indestructible this turn; or target creature gains double strike until end of turn.",
//                    Flavor = "",
//                    Colors = "red;white",
//                    Cost = "RW",
//                    Cmc = 2,
//                    Set_Name = "Gatecrash",
//                    Type = "Instant",
//                    SubType = "",
//                    Power = "0",
//                    Toughness = "0",
//                    Loyalty = "0",
//                    Rarity = "Uncommon",
//                    Artist = "Zoltan Boros",
//                    Set_Id = "GTC",
//                    Token = false,
//                    Img = "https://api.mtgdb.info/content/card_images/366435.jpeg",
//                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/366435.jpg",
//                    Multiverse_Id = 366435,
//                },
//                new Card()
//                {
//                    Name = "Spore Cloud",
//                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
//                    Set_Number = 176,
//                    Cmc = 3,
//                    Cost = "1GG",
//                    Set_Name = "Masters Edition II",
//                    Set_Id = "ME2",
//                    Type = "Instant",
//                    Img = "https://api.mtgdb.info/content/card_images/184710.jpeg",
//                    Multiverse_Id = 184710
//                },
//                new Card()
//                {
//                    Name = "Spore Cloud",
//                    Desc ="Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
//                    Set_Number = 80,
//                    Cmc = 3,
//                    Cost = "1GG",
//                    Set_Name = "Fallen Empires",
//                    Set_Id = "FEM",
//                    Type = "Instant",
//                    Img = "https://api.mtgdb.info/content/card_images/1922.jpeg",
//                    Multiverse_Id = 1922
//                },
//                new Card()
//                {
//                    Id = 7066,
//                    Related_Card_Id = 0,
//                    Set_Number =  1,
//                    Name = "Aerial Maneuver",
//                    Search_Name = "Aerial Maneuver",
//                    Desc = "Target creature gets +1/+1 and gains flying and first strike until end of turn.",
//                    Flavor = "",
//                    Colors = "white",
//                    Cost = "1W",
//                    Cmc = 2,
//                    Set_Name = "Gatecrash",
//                    Type = "Instant",
//                    SubType = "",
//                    Power = "0",
//                    Toughness = "0",
//                    Loyalty = "0",
//                    Rarity = "Common",
//                    Artist = "Scott Chou",
//                    Set_Id = "GTC",
//                    Token = false,
//                    Img = "https://api.mtgdb.info/content/card_images/366294.jpeg",
//                    Img_Hires = "https://api.mtgdb.info/content/hi_res_card_images/366294.jpg",
//                    Multiverse_Id = 366294,
//                },
//            };

//            setData = new List<Set>()
//            {
//                new Set()
//                {
//                    Id = 116,
//                    Name = "Gatecrash",
//                    Code = "GTC",
//                    Block = "Return to Ravnica",
//                    Type = "Expansion",
//                    Desc = "Gatecrash is a Magic: The Gathering expansion set released February 1, 2013. It is the second set of the Return to Ravnica block. The tagline for the set is 'Fight For Your Guild' and it contains 249 cards (101 commons, 80 uncommons, 53 rares, 15 mythic rares). Gatecrash focuses on five of the returning guilds; the Boros Legion, House Dimir, The Orzhov Syndicate, The Gruul Clans, and The Simic Combine. As in the original Ravnica block, Gatecrash focuses on multicolor cards. The storyline told deals with the rise of another faction that does not ally with any of the Guilds. This group is referred to as the 'Gateless'. The Gateless was referred to in the first set in certain cards as well. Another storyline has the tension between the guilds rise, and their attempts to thwart one another.",
//                    CommonQty = 101,
//                    UncommonQty = 80,
//                    RareQty = 53,
//                    MythicQty = 15,
//                    BasicLandQty = 0,
//                    TotalQty = 249,
//                    ReleasedOn = new DateTime(2013,2,1)
//                },
//                new Set()
//                {
//                    Id = 124,
//                    Name = "Commander 2013 Edition",
//                    Code = "C13",
//                    Block = "Commander",
//                    Type = "Non-standard Legal",
//                    Desc = "Commander is a series of five 100-card, three color Magic: the Gathering decks, meant as a supplement to the variant format initially known as 'Elder Dragon Highlander (EDH)'. Each deck is based around a legendary creature, called a 'Commander' or 'General'. No card other than basic lands appear more than once in each deck, and each deck contains three foil oversized legendary creature cards. This set is notable in that it is the first set printed outside of the normal booster pack expansions to have functionally new cards. There are 51 new cards, made specifically for multi-player games, featured in Commander.",
//                    CommonQty = 28,
//                    UncommonQty = 0,
//                    RareQty = 0,
//                    MythicQty = 0,
//                    BasicLandQty = 0,
//                    TotalQty = 28,
//                    ReleasedOn = new DateTime(2013,11,1)
//                },
//            };
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            SetUp_Data();

//            httpClientMock = new Mock<IHttpClient>();
//            loggerMock = new Mock<ILogger>();
//            pluginManagerMock = new Mock<IPluginManager>();
//            messengerMock = new Mock<IMessenger>();

//            // Create a mock set and context
//            cardDbSetMock = new Mock<DbSet<Card>>()
//                .SetupData(cardData);

//            setDbSetMock = new Mock<DbSet<Set>>()
//                .SetupData(setData);

//            contextMock = new Mock<IMtgContext>();
//            contextMock.Setup(c => c.Cards).Returns(cardDbSetMock.Object);
//            contextMock.Setup(c => c.Sets).Returns(setDbSetMock.Object);

//            // Mock IMessenger properties so a null object reference exception is not thrown in IndexModule
//            messengerMock.Setup(p => p.BotName)
//                .Returns("BotName");
//            messengerMock.Setup(p => p.BotId)
//                .Returns("BOTID");

//            // Setup the Browser object
//            //var bootstrapper = new DefaultNancyBootstrapper();
//            browser = new Browser(with =>
//            {
//                with.Module<IndexModule>();
//                with.Dependency<IMtgContext>(contextMock.Object);
//                with.Dependency<IMessenger>(messengerMock.Object);
//                with.Dependency<IPluginManager>(pluginManagerMock.Object);
//            });
//        }

//        [Test]
//        public void ValidMessage()
//        {
//            string groupMeMessageBody = @"{
//""id"":""141909488216484256"",
//""source_guid"":""b4182bb58a18ba162b29434"",
//""created_at"":1419094882,
//""user_id"":""111111"",
//""group_id"":""9999999"",
//""name"":""User Name"",
//""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
//""text"":""This is a test message?"",
//""system"":false,
//""attachments"":[
//]
//}";

//            var response = browser.Post("/",
//            with =>
//            {
//                with.HttpRequest();
//                with.Body(groupMeMessageBody);
//                with.Header("content-type", "application/json");
//            });

//            // Verify that the plugin manager's SendMessage was called and that the message text is what we sent
//            pluginManagerMock.Verify(c => c.SendMessage(It.Is<GroupMeMessage>(m => m.text == "This is a test message?"), It.IsAny<IMessenger>()));

//            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
//        }

//        [Test]
//        public void InvalidMessage()
//        {
//            // This message body is missing the 'groupme_id' and 'text' properties
//            string groupMeMessageBody = @"{
//""id"":""141909488216484256"",
//""source_guid"":""b4182bb58a18ba162b29434"",
//""created_at"":1419094882,
//""user_id"":""111111"",
//""name"":""User Name"",
//""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
//""system"":false,
//""attachments"":[
//]
//}";

//            var response = browser.Post("/",
//            with =>
//            {
//                with.HttpRequest();
//                with.Body(groupMeMessageBody);
//                with.Header("content-type", "application/json");
//            });

//            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
//        }
//    }
//}
