using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nancy;
using Nancy.Testing;
using NerdBot.Admin;
using NerdBot.Modules;
using NerdBotCommon.Http;
using NerdBotCommon.Importer;
using NerdBotCommon.Importer.DataReaders;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NUnit.Framework;
using SimpleLogging.Core;

namespace NerdBot.Tests
{
    [TestFixture]
    class AdminModule_Tests
    {
        private Browser browser;

        private Mock<IMtgStore> storeMock;
        private Mock<IHttpClient> httpClientMock;
        private Mock<ILoggingService> loggerMock;
        private Mock<ICardPriceStore> priceStoreMock;
        private Mock<IMtgDataReader> mtgDataReaderMock;
        private Mock<IImporter> importerMock;

        private List<Card> cardData = new List<Card>();
        private List<Set> setData = new List<Set>();
        private string setAddPostData;

        private void Setup_Data()
        {
            cardData = new List<Card>()
            {
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Charm",
                    SearchName = "boroscharm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control gain indestructible until end of turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "red", "white" },
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Commander 2013 Edition",
                    SetSearchName = "commander2013edition",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    SetId = "C13",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/376270.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/376270.jpg",
                    MultiverseId = 376270,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Charm",
                    SearchName = "boroscharm",
                    Desc = "Choose one - Boros Charm deals 4 damage to target player; or permanents you control are indestructible this turn; or target creature gains double strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "red", "white" },
                    Cost = "RW",
                    Cmc = 2,
                    SetName = "Gatecrash",
                    SetSearchName = "gatecrash",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Zoltan Boros",
                    SetId = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366435.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/366435.jpg",
                    MultiverseId = 366435,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Boros Cluestone",
                    SearchName = "boroscluestone",
                    Desc = "{Tap}: Add {Red} or {White} to your mana pool.\n\r" +
                            "{Red}{White}, {Tap}, Sacrifice Boros Cluestone: Draw a card.",
                    Flavor = "",
                    Colors = new List<string>() { "none" },
                    Cost = "3",
                    Cmc = 3,
                    SetName = "Dragon's Maze",
                    SetSearchName = "dragonsmaze",
                    Type = "Artifact",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Raoul Vitale",
                    SetId = "DGM",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/368997.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/368997.jpg",
                    MultiverseId = 368997,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Spore Cloud",
                    SearchName = "sporecloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Flavor = "",
                    Colors = new List<string>() { "green" },
                    Cost = "1GG",
                    Cmc = 3,
                    SetName = "Masters Edition II",
                    SetSearchName = "masterseditionii",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Uncommon",
                    Artist = "Susan Van Camp",
                    SetId = "ME2",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/184710.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/184710.jpg",
                    MultiverseId = 184710,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Spore Cloud",
                    SearchName = "sporecloud",
                    Desc = "Tap all blocking creatures. Prevent all combat damage that would be dealt this turn. Each attacking creature and each blocking creature doesn't untap during its controller's next untap step.",
                    Flavor = "",
                    Colors = new List<string>() { "green" },
                    Cost = "1GG",
                    Cmc = 3,
                    SetName = "Fallen Empires",
                    SetSearchName = "fallenempires",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Amy Weber",
                    SetId = "FEM",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/1922.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/1922.jpg",
                    MultiverseId = 1922,
                },
                new Card()
                {
                    RelatedCardId = 0,
                    Name = "Aerial Maneuver",
                    SearchName = "aerialmaneuver",
                    Desc = "Target creature gets +1/+1 and gains flying and first strike until end of turn.",
                    Flavor = "",
                    Colors = new List<string>() { "white" },
                    Cost = "1W",
                    Cmc = 2,
                    SetName = "Gatecrash",
                    Type = "Instant",
                    SubType = null,
                    Power = "0",
                    Toughness = "0",
                    Loyalty = "0",
                    Rarity = "Common",
                    Artist = "Scott Chou",
                    SetId = "GTC",
                    Token = false,
                    Img = "https://api.mtgdb.info/content/card_images/366294.jpeg",
                    ImgHires = "https://api.mtgdb.info/content/hi_res_card_images/366294.jpg",
                    MultiverseId = 366294,
                },
            };

            setData = new List<Set>()
            {
                new Set()
                {
                    Name = "Gatecrash",
                    SearchName = "gatecrash",
                    Code = "GTC",
                    Block = "Return to Ravnica",
                    Type = "Expansion",
                    Desc = "Gatecrash is a Magic: The Gathering expansion set released February 1, 2013. It is the second set of the Return to Ravnica block. The tagline for the set is 'Fight For Your Guild' and it contains 249 cards (101 commons, 80 uncommons, 53 rares, 15 mythic rares). Gatecrash focuses on five of the returning guilds; the Boros Legion, House Dimir, The Orzhov Syndicate, The Gruul Clans, and The Simic Combine. As in the original Ravnica block, Gatecrash focuses on multicolor cards. The storyline told deals with the rise of another faction that does not ally with any of the Guilds. This group is referred to as the 'Gateless'. The Gateless was referred to in the first set in certain cards as well. Another storyline has the tension between the guilds rise, and their attempts to thwart one another.",
                    CommonQty = 101,
                    UncommonQty = 80,
                    RareQty = 53,
                    MythicQty = 15,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,2,1)
                },
                new Set()
                {
                    Name = "Commander 2013 Edition",
                    SearchName = "commander2013edition",
                    Code = "C13",
                    Block = "Commander",
                    Type = "Non-standard Legal",
                    Desc = "Commander is a series of five 100-card, three color Magic: the Gathering decks, meant as a supplement to the variant format initially known as 'Elder Dragon Highlander (EDH)'. Each deck is based around a legendary creature, called a 'Commander' or 'General'. No card other than basic lands appear more than once in each deck, and each deck contains three foil oversized legendary creature cards. This set is notable in that it is the first set printed outside of the normal booster pack expansions to have functionally new cards. There are 51 new cards, made specifically for multi-player games, featured in Commander.",
                    CommonQty = 28,
                    UncommonQty = 0,
                    RareQty = 0,
                    MythicQty = 0,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,11,1)
                },
                new Set()
                {
                    Name = "Dragon's Maze",
                    SearchName = "dragonsmaze",
                    Code = "DGM",
                    Block = "Return to Ravnica",
                    Type = "Expansion",
                    Desc = "Dragon's Maze is a Magic: The Gathering expansion set that was released on May 3, 2013. It is the third set of the Return to Ravnica block and contains 156 cards. All ten guilds of Ravnica will be included in the set. As in the original Ravnica block, Dragon's Maze focuses on multi-color cards. The set also marks the culmination of the Izzet League's research into the depths of Ravnica, the eponymous Dragon's Maze -- A path that treads all 10 guild gates in order to find and activate an energy source of immense power which is enough to subjugate and control all of the guilds. The Izzet propose a challenge in which each guild selects a champion in order to navigate and conquer the maze and subsequently the other guilds. Each Champion will have to traverse the maze, and they will have to deal with other guilds' attempts to halt their advance.",
                    CommonQty = 70,
                    UncommonQty = 40,
                    RareQty = 35,
                    MythicQty = 11,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(2013,5,3)
                },
                new Set()
                {
                    Name = "Masters Edition II",
                    SearchName = "masterseditionii",
                    Code = "ME2",
                    Block = "Masters Editions",
                    Type = "Online",
                    Desc = @"The _Masters Edition II_ set is a collection of 
  _Magic_(TM) cards that were originally printed before the _Mirage_(TM) set was
  released. It's a 245-card, black-bordered set featuring 80 rares, 
  80 uncommons, 80 commons, and 5 basic lands. The _Masters Edition II_ set is 
  nonredeemable: Online cards from the _Masters Edition II_ set can't be 
  exchanged for physical cards",
                    CommonQty = 80,
                    UncommonQty = 80,
                    RareQty = 80,
                    MythicQty = 0,
                    BasicLandQty = 5,
                    ReleasedOn = new DateTime(2008,9,26)
                },
                new Set()
                {
                    Name = "Fallen Empires",
                    SearchName = "fallenempires",
                    Code = "FEM",
                    Block = null,
                    Type = "Expansion",
                    Desc = null,
                    CommonQty = 121,
                    UncommonQty = 30,
                    RareQty = 36,
                    MythicQty = 0,
                    BasicLandQty = 0,
                    ReleasedOn = new DateTime(1994,1,11)
                },
            };

            setAddPostData = @"{  
   ""name"":""Hour of Devastation"",
   ""code"":""HOU"",
   ""releaseDate"":""2017-07-14"",
   ""border"":""black"",
   ""block"":""Amonkhet"",
   ""type"":""expansion"",
   ""booster"":[  
      [  
         ""rare"",
         ""mythic rare""
      ],
      ""uncommon"",
      ""uncommon"",
      ""uncommon"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""common"",
      ""land"",
      ""marketing""
   ],
   ""cards"":[  
      {  
         ""artist"":""Magali Villeneuve"",
         ""cmc"":2,
         ""colorIdentity"":[  
            ""W""
         ],
         ""colors"":[  
            ""White""
         ],
         ""flavor"":""\""On every plane, there are those who run toward danger.\""\n—Gideon Jura"",
         ""foreignNames"":[  
            {  
               ""language"":""Chinese Simplified"",
               ""name"":""英勇义举"",
               ""multiverseid"":432481
            },
            {  
               ""language"":""Chinese Traditional"",
               ""name"":""英勇義舉"",
               ""multiverseid"":432680
            },
            {  
               ""language"":""French"",
               ""name"":""Acte héroïque"",
               ""multiverseid"":431287
            },
            {  
               ""language"":""German"",
               ""name"":""Heldentat"",
               ""multiverseid"":430889
            },
            {  
               ""language"":""Italian"",
               ""name"":""Atto di Eroismo"",
               ""multiverseid"":431486
            },
            {  
               ""language"":""Japanese"",
               ""name"":""英雄的行動"",
               ""multiverseid"":431685
            },
            {  
               ""language"":""Korean"",
               ""name"":""영웅적인 행동"",
               ""multiverseid"":431884
            },
            {  
               ""language"":""Portuguese (Brazil)"",
               ""name"":""Ato de Heroísmo"",
               ""multiverseid"":432083
            },
            {  
               ""language"":""Russian"",
               ""name"":""Героический Подвиг"",
               ""multiverseid"":432282
            },
            {  
               ""language"":""Spanish"",
               ""name"":""Acto de heroísmo"",
               ""multiverseid"":431088
            }
         ],
         ""id"":""83e36c2e95707b969774480aec06acf90fb2435e"",
         ""imageName"":""act of heroism"",
         ""layout"":""normal"",
         ""legalities"":[  
            {  
               ""format"":""Amonkhet Block"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Commander"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Legacy"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Modern"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Standard"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Vintage"",
               ""legality"":""Legal""
            }
         ],
         ""manaCost"":""{1}{W}"",
         ""multiverseid"":430690,
         ""name"":""Act of Heroism"",
         ""number"":""1"",
         ""originalText"":""Untap target creature. It gets +2/+2 until end of turn and can block an additional creature this turn."",
         ""originalType"":""Instant"",
         ""printings"":[  
            ""HOU""
         ],
         ""rarity"":""Common"",
         ""rulings"":[  
            {  
               ""date"":""2017-06-27"",
               ""text"":""You can cast Act of Heroism even if the target creature won't be able to block right away, perhaps because you're the attacking player.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""Untapping an attacking creature doesn't remove it from combat.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""Act of Heroism can target an untapped creature. It still gets +2/+2 and can block an additional creature.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""The effects of Act of Heroism are cumulative. If multiples resolve targeting the same creature, that creature can block that many additional creatures this turn.""
            }
         ],
         ""text"":""Untap target creature. It gets +2/+2 until end of turn and can block an additional creature this turn."",
         ""type"":""Instant"",
         ""types"":[  
            ""Instant""
         ]
      },
      {  
         ""artist"":""Slawomir Maniak"",
         ""cmc"":2,
         ""colorIdentity"":[  
            ""W""
         ],
         ""colors"":[  
            ""White""
         ],
         ""foreignNames"":[  
            {  
               ""language"":""Chinese Simplified"",
               ""name"":""佩饰扑击猫"",
               ""multiverseid"":432482
            },
            {  
               ""language"":""Chinese Traditional"",
               ""name"":""佩飾撲擊貓"",
               ""multiverseid"":432681
            },
            {  
               ""language"":""French"",
               ""name"":""Bondisseur paré"",
               ""multiverseid"":431288
            },
            {  
               ""language"":""German"",
               ""name"":""Geschmückte Raubkatze"",
               ""multiverseid"":430890
            },
            {  
               ""language"":""Italian"",
               ""name"":""Assalitore Ornato"",
               ""multiverseid"":431487
            },
            {  
               ""language"":""Japanese"",
               ""name"":""典雅な襲撃者"",
               ""multiverseid"":431686
            },
            {  
               ""language"":""Korean"",
               ""name"":""치장한 맹수"",
               ""multiverseid"":431885
            },
            {  
               ""language"":""Portuguese (Brazil)"",
               ""name"":""Saltador Adornado"",
               ""multiverseid"":432084
            },
            {  
               ""language"":""Russian"",
               ""name"":""Украшенная Хищница"",
               ""multiverseid"":432283
            },
            {  
               ""language"":""Spanish"",
               ""name"":""Acechador ornamentado"",
               ""multiverseid"":431089
            }
         ],
         ""id"":""13b2e582c6074c5df03865000291b2a66f76ce1c"",
         ""imageName"":""adorned pouncer"",
         ""layout"":""normal"",
         ""legalities"":[  
            {  
               ""format"":""Amonkhet Block"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Commander"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Legacy"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Modern"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Standard"",
               ""legality"":""Legal""
            },
            {  
               ""format"":""Vintage"",
               ""legality"":""Legal""
            }
         ],
         ""manaCost"":""{1}{W}"",
         ""multiverseid"":430691,
         ""name"":""Adorned Pouncer"",
         ""number"":""2"",
         ""originalText"":""Double strike\nEternalize {3}{W}{W} ({3}{W}{W}, Exile this card from your graveyard: Create a token that's a copy of it, except it's a 4/4 black Zombie Cat with no mana cost. Eternalize only as a sorcery.)"",
         ""originalType"":""Creature — Cat"",
         ""power"":""1"",
         ""printings"":[  
            ""HOU""
         ],
         ""rarity"":""Rare"",
         ""rulings"":[  
            {  
               ""date"":""2017-06-27"",
               ""text"":""For each card with eternalize, a corresponding game play supplement token can be found in some Hour of Devastation booster packs. These supplements are not required to play with cards with eternalize; you can use the same items to represent an eternalized token as you would any other token.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""If a creature card with eternalize is put into your graveyard during your main phase, you'll have priority immediately afterward. You can activate its eternalize ability before any player can try to exile it, such as with Crook of Condemnation, if it's legal for you to do so.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""Once you've activated an eternalize ability, the card is immediately exiled. Opponents can't try to stop the ability by exiling the card with an effect such as that of Crook of Condemnation.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""The token copies exactly what was printed on the original card and nothing else, except the characteristics specifically modified by eternalize. It doesn't copy any information about the object the card was before it was put into your graveyard.""
            },
            {  
               ""date"":""2017-06-27"",
               ""text"":""The token is a Zombie in addition to its other types and is black instead of its other colors. Its base power and toughness are 4/4. It has no mana cost, and thus its converted mana cost is 0. These are copiable values of the token that other effects may copy.""
            }
         ],
         ""subtypes"":[  
            ""Cat""
         ],
         ""text"":""Double strike\nEternalize {3}{W}{W} ({3}{W}{W}, Exile this card from your graveyard: Create a token that's a copy of it, except it's a 4/4 black Zombie Cat with no mana cost. Eternalize only as a sorcery.)"",
         ""toughness"":""1"",
         ""type"":""Creature — Cat"",
         ""types"":[  
            ""Creature""
         ]
      }
   ]
}";
        }

        [SetUp]
        public void SetUp()
        {
            Setup_Data();

            httpClientMock = new Mock<IHttpClient>();
            loggerMock = new Mock<ILoggingService>();
            priceStoreMock = new Mock<ICardPriceStore>();
            mtgDataReaderMock = new Mock<IMtgDataReader>();
            importerMock = new Mock<IImporter>();

            // Create a mock set and IMtgStore
            storeMock = new Mock<IMtgStore>();

            // Setup the Browser object
            browser = new Browser(with =>
            {
                with.Module<AdminModule>();
                with.RequestStartup((container, pipelines, context) =>
                {
                    context.CurrentUser = new AuthenticatedUser { UserName = "admin" };
                });
                with.Dependency<IMtgStore>(storeMock.Object);
                with.Dependency<ILoggingService>(loggerMock.Object);
                with.Dependency<ICardPriceStore>(priceStoreMock.Object);
                with.Dependency<IMtgDataReader>(mtgDataReaderMock.Object);
                with.Dependency<IHttpClient>(httpClientMock.Object);
                with.Dependencies<IImporter>(importerMock.Object);
            });
        }

        [Test]
        public void ListSets()
        {
            storeMock.Setup(s => s.GetSets())
                .ReturnsAsync(setData);

            var response = browser.Get("/admin/sets",
            with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ListCardsBySet()
        {
            storeMock.Setup(s => s.GetSetByCode("GTC"))
                .ReturnsAsync(setData.Where(s => s.Code == "GTC").FirstOrDefault());

            storeMock.Setup(s => s.GetCardsBySet("Gatecrash"))
                .ReturnsAsync(cardData.Where(c => c.SetId == "GTC").ToList());

            var response = browser.Get("/admin/set/GTC",
            with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void AddSetPage()
        {
            var response = browser.Get("/admin/addset",
            with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void AddSet()
        {
            string postData = @" { ""url"" : ""http://localhost/test.json"" }";

            var set = setData.Where(s => s.Code == "GTC").FirstOrDefault();
            var cards = cardData.Where(c => c.SetId == "GTC").AsEnumerable<Card>();

            httpClientMock.Setup(h => h.GetAsJson("http://localhost/test.json"))
                .ReturnsAsync(setAddPostData);

            mtgDataReaderMock.Setup(d => d.ReadSet(setAddPostData))
                .Returns(() => set);

            mtgDataReaderMock.Setup(d => d.ReadCards(setAddPostData))
                .Returns(() => cards);

            importerMock.Setup(i => i.Import(set, cards))
                .ReturnsAsync(new ImportStatus()
                {
                    ImportedSet = set,
                    ImportedCards = cards.ToList(),
                    FailedCards = new List<Card>()
                });

            var response = browser.Post("/admin/addset",
            with =>
            {
                with.HttpRequest();
                with.Body(postData);
                with.Header("content-type", "application/json");
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
