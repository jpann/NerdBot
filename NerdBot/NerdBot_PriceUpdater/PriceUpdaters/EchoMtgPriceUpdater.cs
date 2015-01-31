using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using SimpleLogging.Core;

namespace NerdBot_PriceUpdater.PriceUpdaters
{
    public class EchoMtgPriceUpdater : IPriceUpdater
    {
        private readonly IMtgStore mMtgStore;
        private readonly ICardPriceStore mPriceStore;
        private readonly ILoggingService mLoggingService;

        public EchoMtgPriceUpdater(IMtgStore mtgStore, ICardPriceStore priceStore, ILoggingService loggingService)
        {
            if (mtgStore == null)
                throw new ArgumentNullException("mtgStore");

            if (priceStore == null)
                throw new ArgumentNullException("priceStore");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this.mMtgStore = mtgStore;
            this.mPriceStore = priceStore;
            this.mLoggingService = loggingService;

        }

        public void UpdatePrices(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
