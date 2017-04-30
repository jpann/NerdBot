﻿using System;
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
        private string mImageUrl;
        private string mImageHiResUrl;

        #region Properties
        public string ImageUrl
        {
            get { return this.mImageUrl; }
            set
            {
                if (!value.Contains("{0}") && !value.Contains("{1}"))
                    throw new FormatException("ImageUrl must contain {0} (set code) and {1} (multiverse id) format arguments.");

                this.mImageUrl = value;
            }
        }

        public string ImageHiResUrl
        {
            get { return this.mImageHiResUrl; }
            set
            {
                if (!value.Contains("{0}") && !value.Contains("{1}"))
                    throw new FormatException("ImageUrl must contain {0} (set code) and {1} (multiverse id) format arguments.");

                this.mImageHiResUrl = value;
            }
        }
        #endregion

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

            if (source.Colors != null)
                card.Colors = source.Colors.ToList();

            card.Cost = source.ManaCost;
            card.Desc = source.Text;
            card.Flavor = source.Flavor;
            card.Img = string.Format(this.mImageUrl, setCode, source.MultiverseId);
            card.ImgHires = string.Format(this.ImageHiResUrl, setCode, source.MultiverseId);
            card.Loyalty = source.Loyalty;
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

            if (source.Rulings != null)
            {
                List<Ruling> rulings = new List<Ruling>();
                foreach (MtgJsonRuling r in source.Rulings)
                {
                    Ruling ruling = new Ruling()
                    {
                        ReleasedOn = r.Date,
                        Rule = r.Text
                    };

                    rulings.Add(ruling);
                }
                card.Rulings = rulings;
            }

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
            set.BasicLandQty = source.BasicLandQty;
            set.CommonQty = source.CommonQty;
            set.MythicQty = source.MythicQty;
            set.UncommonQty = source.UncommonQty;
            set.RareQty = source.RareQty;
            set.SearchName = this.mSearchUtility.GetSearchValue(source.Name);

            return set;
        }

    }
}
