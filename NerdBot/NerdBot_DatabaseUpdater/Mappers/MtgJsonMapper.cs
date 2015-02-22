using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdBot.Mtg;
using NerdBot.Utilities;
using NerdBot_DatabaseUpdater.MtgData;

namespace NerdBot_DatabaseUpdater.Mappers
{
    public class MtgJsonMapper : IMtgDataMapper<MtgJsonCard, MtgJsonSet>
    {
        private readonly SearchUtility mSearchUtility;

        public MtgJsonMapper(SearchUtility searchUtility)
        {
            if (searchUtility == null)
                throw  new ArgumentNullException("searchUtility");

            this.mSearchUtility = searchUtility;
        }

        public Card GetCard(MtgJsonCard source)
        {
            throw new NotImplementedException();
        }

        public Card GetCard(MtgJsonCard source, string setName, string setCode)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            if (string.IsNullOrEmpty(setCode))
                throw new ArgumentException("setCode");

            Card card = new Card();

            card.Artist = source.Artist;
            card.Cmc = source.CMC;
            card.Colors = source.Colors;
            card.Cost = source.ManaCost;
            card.Desc = source.Text;
            card.Flavor = source.Flavor;
            card.Img = string.Format("http://mtgimage.com/multiverseid/{0}.jpg", source.MultiverseId);
            card.ImgHires = string.Format("http://mtgimage.com/multiverseid/{0}.jpg", source.MultiverseId);
            card.Loyalty = source.Loyalty.ToString();
            card.MultiverseId = Convert.ToInt32(source.MultiverseId);
            card.Name = source.Name;
            card.SearchName = this.mSearchUtility.GetSearchValue(source.Name);
            card.SetId = setCode;
            card.SetName = setName;
            card.SetSearchName = this.mSearchUtility.GetSearchValue(setName);
            card.Power = source.Power;
            card.Rarity = source.Rarity;

            if (source.SubTypes != null)
                card.SubType = string.Join(" ", source.SubTypes.ToArray());

            card.Token = false;
            card.Toughness = source.Toughness;
            card.Type = string.Join(" ", source.Types.ToArray());

            return card;
        }

        public Set GetSet(MtgJsonSet source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Set set = new Set();

            set.Block = source.Block;
            set.Type = source.Type;
            set.Code = source.Code;
            set.Name = source.Name;
            set.ReleasedOn = source.ReleaseDate;
            set.BasicLandQty = source.Cards.Count(c => c.Rarity.Contains("Basic Land"));
            set.CommonQty = source.Cards.Count(c => c.Rarity.Contains("Common"));
            set.MythicQty = source.Cards.Count(c => c.Rarity.Contains("Mythic"));
            set.UncommonQty = source.Cards.Count(c => c.Rarity.Contains("Uncommon"));
            set.RareQty = source.Cards.Count(c => c.Rarity.Contains("Rare"));
            set.SearchName = this.mSearchUtility.GetSearchValue(source.Name);

            return set;
        }

    }
}
