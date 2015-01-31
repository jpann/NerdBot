using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot_PriceUpdater.PriceUpdaters
{
    public interface IPriceUpdater
    {
        void UpdatePrices(object sender, DoWorkEventArgs e);
    }
}
