using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NerdBot.Mtg;

namespace NerdBot_PriceUpdater.PriceUpdaters
{
    public interface IPriceUpdater
    {
        void UpdatePrices(Set set);
    }
}
