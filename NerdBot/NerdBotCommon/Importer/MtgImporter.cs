using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Threading.Tasks;
using NerdBotCommon.Mtg;
using SimpleLogging.Core;

namespace NerdBotCommon.Importer
{
    public class MtgImporter : IImporter
    {
        private IMtgStore mStore;
        private ILoggingService mLoggingService;

        public MtgImporter(IMtgStore store, ILoggingService loggingService)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mStore = store;
            this.mLoggingService = loggingService;
        }

        public async Task<ImportStatus> Import(Set set, IEnumerable<Card> cards)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            if (cards == null)
                throw new ArgumentNullException("cards");

            var status = new ImportStatus();

            var setInserted = this.mStore.SetFindAndModify(set);

            if (setInserted.Result == null)
            {
                this.mLoggingService.Warning("Set '{0}' not inserted.", set.Name);

                return null;
            }

            status.ImportedSet = setInserted.Result;

            List<Card> inserted_Cards = new List<Card>();
            List<Card> failed_Cards = new List<Card>();

            foreach (Card card in cards)
            {
                this.mLoggingService.Debug("Read card '{0}'...", card.Name);

                var cardInserted = this.mStore.CardFindAndModify(card);

                if (cardInserted.Result != null)
                {
                    this.mLoggingService.Info("Card '{0}' in set '{1}' inserted!",
                        cardInserted.Result.Name,
                        setInserted.Result.Name);

                    inserted_Cards.Add(cardInserted.Result);
                }
                else
                {
                    this.mLoggingService.Warning("Card '{0}' in set '{1}' failed to inserted!",
                        card.Name,
                        setInserted.Result.Name);

                    failed_Cards.Add(card);
                }
            }

            status.ImportedCards = inserted_Cards;
            status.FailedCards = failed_Cards;

            return status;
        }
    }

    public class ImportStatus
    {
        public Set ImportedSet { get; set; }
        public List<Card> ImportedCards { get; set; }
        public List<Card> FailedCards { get; set; }
    }
}