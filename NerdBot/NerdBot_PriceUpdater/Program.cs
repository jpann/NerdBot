using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot_PriceUpdater.PriceUpdaters;
using SimpleLogging.Core;
using TinyIoC;

namespace NerdBot_PriceUpdater
{
    class Program
    {
        private static ILoggingService mLoggingService;
        static void Main(string[] args)
        {
            Bootstrapper.Register();

            mLoggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();

            var priceUpdater = TinyIoCContainer.Current.Resolve<EchoMtgPriceUpdater>();

            Console.ReadKey();
        }
    }
}
