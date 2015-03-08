using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NerdBot.Mtg;
using NerdBot.Utilities;

namespace NerdBot_DatabaseUpdater.Mappers
{
    public class MtgInfoMapper : IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet>
    {
        private readonly SearchUtility mSearchUtility;

        public MtgInfoMapper(SearchUtility searchUtility)
        {
            if (searchUtility == null)
                throw  new ArgumentNullException("searchUtility");

            this.mSearchUtility = searchUtility;
        }

        public Card GetCard(MtgDb.Info.Card source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Card card = new Card();
            card.Artist = source.Artist;
            card.Cmc = source.ConvertedManaCost;
            card.Colors = source.Colors.ToList();
            card.Desc = source.Description;
            card.Flavor = source.Flavor;
            card.Img = source.CardImage;
            card.ImgHires = source.ImageHiRes;
            card.Loyalty = source.Loyalty.ToString();
            card.MultiverseId = source.Id;
            card.Name = source.Name;
            card.Power = source.Power.ToString();
            card.Rarity = source.Rarity;
            card.RelatedCardId = source.RelatedCardId;

            List<Ruling> rulings = new List<Ruling>();
            foreach (MtgDb.Info.Ruling r in source.Rulings)
            {
                Ruling ruling = new Ruling()
                {
                    ReleasedOn = r.ReleasedAt,
                    Rule = r.Rule
                };

                rulings.Add(ruling);
            }
            card.Rulings = rulings;

            card.SearchName = this.mSearchUtility.GetSearchValue(source.Name);
            card.SetId = source.CardSetId;
            card.SetName = source.CardSetName;
            card.SetSearchName = this.mSearchUtility.GetSearchValue(source.CardSetName);
            card.SetNumber = source.SetNumber;
            card.SubType = source.SubType;
            card.Token = source.Token;
            card.Toughness = source.Toughness.ToString();
            card.Type = source.Type;

            return card;
        }

        public Card GetCard(MtgDb.Info.Card source, string setName, string setCode)
        {
            throw new NotImplementedException();
        }

        public Set GetSet(MtgDb.Info.CardSet source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Set set = new Set();

            set.BasicLandQty = source.BasicLand;
            set.Block = source.Block;
            set.Code = source.Id;
            set.CommonQty = source.Common;
            set.Desc = source.Description;
            set.MythicQty = source.MythicRare;
            set.Name = source.Name;
            set.RareQty = source.Rare;
            set.ReleasedOn = source.ReleasedAt;
            set.Type = source.Type;
            set.UncommonQty = source.Uncommon;
            set.SearchName = this.mSearchUtility.GetSearchValue(source.Name);

            return set;
        }
    }
}
