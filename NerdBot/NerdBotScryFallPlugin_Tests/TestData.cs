using System;
using NerdBotCommon.Mtg;

namespace NerdBotScryFallPlugin_Tests
{
    public class TestData
    {
        public Card TestCard { get; set; }
        public string TestCardPng_Json { get; set; }
        public string TestCardLarge_Json { get; set; }
        public string TestCardNormal_Json { get; set; }
        public string TestCardSmall_Json { get; set; }
        public string TestCardNoImage_Json { get; set; }
        public string TestCardNoUSDPrice_Json { get; set; }
        public Card TestCard_ScryNotFound { get; set; }

        public TestData()
        {
            TestCard = new Card()
            {
                Name = "Strip Mine",
                MultiverseId = 409574,
                SearchName = "stripmine",
                SetSearchName = "zendikarexpeditions",
                SetName = "Zendikar Expeditions",
                SetId = "EXP",
                Img = "http://localhost/cards/409574.jpg"
            };

            TestCardPng_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": ""https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"",
    ""normal"": ""https://img.scryfall.com/cards/normal/en/exp/43.jpg?1509690533"",
    ""large"": ""https://img.scryfall.com/cards/large/en/exp/43.jpg?1509690533"",
    ""png"": ""https://img.scryfall.com/cards/png/en/exp/43.png?1509690533"",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": ""53.54"",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";

            TestCardLarge_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": ""https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"",
    ""normal"": ""https://img.scryfall.com/cards/normal/en/exp/43.jpg?1509690533"",
    ""large"": ""https://img.scryfall.com/cards/large/en/exp/43.jpg?1509690533"",
    ""png"": """",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": ""53.54"",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";

            TestCardNormal_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": ""https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"",
    ""normal"": ""https://img.scryfall.com/cards/normal/en/exp/43.jpg?1509690533"",
    ""large"": """",
    ""png"": """",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": ""53.54"",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";
            TestCardSmall_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": ""https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"",
    ""normal"": """",
    ""large"": """",
    ""png"": """",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": ""53.54"",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";

            TestCardNoImage_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": """",
    ""normal"": """",
    ""large"": """",
    ""png"": """",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": ""53.54"",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";

            TestCardNoUSDPrice_Json = @"{
  ""object"": ""card"",
  ""id"": ""1b8de79d-5efe-4e21-8614-786689fcad58"",
  ""multiverse_ids"": [
    409574
  ],
  ""mtgo_id"": 59695,
  ""mtgo_foil_id"": 59696,
  ""name"": ""Strip Mine"",
  ""uri"": ""https://api.scryfall.com/cards/exp/43"",
  ""scryfall_uri"": ""https://scryfall.com/card/exp/43?utm_source=api"",
  ""layout"": ""normal"",
  ""highres_image"": true,
  ""image_uris"": {
    ""small"": ""https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"",
    ""normal"": ""https://img.scryfall.com/cards/normal/en/exp/43.jpg?1509690533"",
    ""large"": ""https://img.scryfall.com/cards/large/en/exp/43.jpg?1509690533"",
    ""png"": ""https://img.scryfall.com/cards/png/en/exp/43.png?1509690533"",
    ""art_crop"": ""https://img.scryfall.com/cards/art_crop/en/exp/43.jpg?1509690533"",
    ""border_crop"": ""https://img.scryfall.com/cards/border_crop/en/exp/43.jpg?1509690533""
  },
  ""cmc"": 0,
  ""type_line"": ""Land"",
  ""oracle_text"": ""{T}: Add {C} to your mana pool.\n{T}, Sacrifice Strip Mine: Destroy target land."",
  ""mana_cost"": """",
  ""colors"": [],
  ""color_identity"": [],
  ""legalities"": {
    ""standard"": ""not_legal"",
    ""frontier"": ""not_legal"",
    ""modern"": ""not_legal"",
    ""pauper"": ""not_legal"",
    ""legacy"": ""banned"",
    ""penny"": ""not_legal"",
    ""vintage"": ""restricted"",
    ""duel"": ""banned"",
    ""commander"": ""legal"",
    ""1v1"": ""banned"",
    ""future"": ""not_legal""
  },
  ""reserved"": false,
  ""reprint"": true,
  ""set"": ""exp"",
  ""set_name"": ""Zendikar Expeditions"",
  ""set_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""set_search_uri"": ""https://api.scryfall.com/cards/search?q=%2B%2Be%3Aexp"",
  ""scryfall_set_uri"": ""https://scryfall.com/sets/exp?utm_source=api"",
  ""rulings_uri"": ""https://api.scryfall.com/cards/exp/43/rulings"",
  ""prints_search_uri"": ""https://api.scryfall.com/cards/search?order=set&q=%2B%2B%21%22Strip+Mine%22"",
  ""collector_number"": ""43"",
  ""digital"": false,
  ""rarity"": ""mythic"",
  ""illustration_id"": ""4729ec53-1be1-4c3f-ab58-b456c96ac8b0"",
  ""artist"": ""Howard Lyon"",
  ""frame"": ""2015"",
  ""full_art"": false,
  ""border_color"": ""black"",
  ""timeshifted"": false,
  ""colorshifted"": false,
  ""futureshifted"": false,
  ""edhrec_rank"": 58,
  ""usd"": """",
  ""tix"": ""1.24"",
  ""eur"": ""42.46"",
  ""related_uris"": {
    ""gatherer"": ""http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=409574"",
    ""tcgplayer_decks"": ""http://decks.tcgplayer.com/magic/deck/search?contains=Strip+Mine&page=1&partner=Scryfall"",
    ""edhrec"": ""http://edhrec.com/route/?cc=Strip+Mine"",
    ""mtgtop8"": ""http://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Strip+Mine""
  },
  ""purchase_uris"": {
    ""amazon"": ""https://www.amazon.com/gp/search?ie=UTF8&index=toys-and-games&keywords=Strip+Mine&tag=scryfall-20"",
    ""ebay"": ""http://rover.ebay.com/rover/1/711-53200-19255-0/1?campid=5337966903&icep_catId=19107&icep_ff3=10&icep_sortBy=12&icep_uq=Strip+Mine&icep_vectorid=229466&ipn=psmain&kw=lg&kwid=902099&mtid=824&pub=5575230669&toolid=10001"",
    ""tcgplayer"": ""http://store.tcgplayer.com/magic/zendikar-expeditions/strip-mine?partner=Scryfall"",
    ""magiccardmarket"": ""https://www.cardmarket.com/Magic/Products/Singles/Zendikar+Expeditions/Strip+Mine?referrer=scryfall"",
    ""cardhoarder"": ""https://www.cardhoarder.com/cards/59695?affiliate_id=scryfall&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"",
    ""card_kingdom"": ""https://www.cardkingdom.com/catalog/item/204745?partner=scryfall&utm_campaign=affiliate&utm_medium=scryfall&utm_source=scryfall"",
    ""mtgo_traders"": ""http://www.mtgotraders.com/deck/ref.php?id=59695&referral=scryfall"",
    ""coolstuffinc"": ""http://www.coolstuffinc.com/p/Magic%3A+The+Gathering/Strip+Mine?utm_source=scryfall""
  }
}";


            TestCard_ScryNotFound = new Card()
            {
                Name = "Spore Cloud",
                MultiverseId = 409571,
                SearchName = "sporecloud",
                SetSearchName = "zendikarexpeditions",
                SetName = "Zendikar Expeditions",
                SetId = "EXP",
                Img = "http://localhost/cards/409571.jpg"
            };
        }

        public string UriEscape(string value)
        {
            return Uri.EscapeDataString(value);
        }
    }
}