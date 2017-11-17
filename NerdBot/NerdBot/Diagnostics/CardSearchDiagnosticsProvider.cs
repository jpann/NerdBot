using Nancy.Diagnostics;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;

namespace NerdBot.Diagnostics
{
    public class CardSearchDiagnosticsProvider : IDiagnosticsProvider
    {
        private readonly IMtgStore mMtgStore;
        private readonly ICardPriceStore mPriceStore;

        public string Name
        {
            get { return "Card Searching Diagnostics Provider"; }
        }

        public string Description
        {
            get { return "Provides diagnostics for quicky checking card search results."; }
        }

        public object DiagnosticObject
        {
            get { return this; }
        }

        public CardSearchDiagnosticsProvider(IMtgStore store, ICardPriceStore priceStore)
        {
            this.mMtgStore = store;
            this.mPriceStore = priceStore;
        }

        [Description("Get card image")]
        public string GetCardImage(string searchString)
        {
            var card = this.mMtgStore.GetCard(searchString).Result;

            if (card != null)
            {
                return card.Img;
            }

            return "NOT FOUND";
        }
    }
}