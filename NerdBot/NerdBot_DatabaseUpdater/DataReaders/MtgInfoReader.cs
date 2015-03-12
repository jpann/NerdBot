using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using MtgDb.Info.Driver;
using NerdBot.Mtg;
using NerdBot_DatabaseUpdater.Mappers;
using SimpleLogging.Core;

namespace NerdBot_DatabaseUpdater.DataReaders
{
    public class MtgInfoReader : IMtgDataReader
    {
        private readonly IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet> mDataMapper;
        private readonly ILoggingService mLoggingService;
        private readonly Db mMtgInfoDb;
        private readonly string mSetId;

        public MtgInfoReader(
            string setId,
            Db mtgInfoDb,
            IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet> dataMapper,
            ILoggingService loggingService)
        {
            if (string.IsNullOrEmpty(setId))
                throw new ArgumentException("setName");

            if (mtgInfoDb == null)
                throw new ArgumentNullException("mtgInfoDb");

            if (dataMapper == null)
                throw new ArgumentNullException("dataMapper");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mSetId = setId;
            this.mMtgInfoDb = mtgInfoDb;
            this.mDataMapper = dataMapper;
            this.mLoggingService = loggingService;
        }

        public Set ReadSet()
        {
            this.mLoggingService.Debug("Reading set '{0}'...", this.mSetId);
            MtgDb.Info.CardSet setData = this.mMtgInfoDb.GetSet(this.mSetId);
            this.mLoggingService.Debug("Read set!");

            if (setData == null)
                throw new Exception(string.Format("Set not found '{0}'.", this.mSetId));

            this.mLoggingService.Debug("Mapping data...");
            var set = this.mDataMapper.GetSet(setData);
            this.mLoggingService.Debug("Data mapped for set '{0}'!", setData.Name);

            return set;
        }

        public IEnumerable<Card> ReadCards()
        {
            this.mLoggingService.Debug("Reading cards from set '{0}'...", this.mSetId);
            MtgDb.Info.Card[] cardsData = this.mMtgInfoDb.GetSetCards(this.mSetId);
            this.mLoggingService.Debug("Read cards!");

            // Some sets do not contain any cards. For example, 'ATH' from MtgInfoDB contains no cards.
            if (cardsData == null || !cardsData.Any())
            {
                this.mLoggingService.Warning("Cards not found in set '{0}'.", this.mSetId);
            }

            this.mLoggingService.Trace("{0} cards found in set '{0}'.", 
                cardsData.Length,
                this.mSetId);

            // Go through each card
            foreach (MtgDb.Info.Card cardData in cardsData)
            {
                this.mLoggingService.Debug("Mapping data...");
                var card = this.mDataMapper.GetCard(cardData);
                this.mLoggingService.Debug("Data mapped for card '{0}'!", cardData.Name);

                yield return card;
            }
        }
    }
}
