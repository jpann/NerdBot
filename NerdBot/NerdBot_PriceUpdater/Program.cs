using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot_PriceUpdater.PriceUpdaters;
using SimpleLogging.Core;
using TinyIoC;

namespace NerdBot_PriceUpdater
{
    internal class Program
    {
        private static ILoggingService mLoggingService;
        private static EchoMtgPriceUpdater mEchoMtgPriceUpdater;
        private static BackgroundWorker mEchoMtgBackgroundWorker;

        private static void Main(string[] args)
        {
            Bootstrapper.Register();

            mLoggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();

            mEchoMtgPriceUpdater = TinyIoCContainer.Current.Resolve<EchoMtgPriceUpdater>();

            // Setup worker
            mEchoMtgBackgroundWorker = new BackgroundWorker();
            mEchoMtgBackgroundWorker.WorkerReportsProgress = true;
            mEchoMtgBackgroundWorker.WorkerSupportsCancellation = true;
            mEchoMtgBackgroundWorker.DoWork += new DoWorkEventHandler(mEchoMtgPriceUpdater.UpdatePrices);
            mEchoMtgBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_EchoMtgsCompleted);
            mEchoMtgBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(bw_EchoMtgProgressChanged);

            try
            {
                Console.WriteLine("Running worker thread for EchoMtg...");
                mEchoMtgBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception er)
            {
                string msg = string.Format("Error: {0}", er.Message);
                
                mLoggingService.Fatal(er, msg);
                Console.WriteLine(msg);
            }

            Console.ReadKey();
        }

        private static void bw_EchoMtgsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                string sMsg = string.Format("Cancelled. {0}", e.Result);
                
                mLoggingService.Warning(sMsg);
                Console.WriteLine(sMsg);
            }
            else if ((e.Error != null))
            {
                string sMsg = string.Format("Error: {0}", e.Error.Message);

                mLoggingService.Error(e.Error, sMsg);
                Console.WriteLine(sMsg);
            }
            else
            {
                string sMsg = string.Format("Completed. {0}", e.Result);

                mLoggingService.Info(sMsg);
                Console.WriteLine(sMsg);
            }
        }

        private static void bw_EchoMtgProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
    }
}
