using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NerdBot_PriceUpdater.PriceUpdaters;
using SimpleLogging.Core;
using TinyIoC;

namespace NerdBot_PriceUpdater
{
    internal class Program
    {
        private static IMtgStore mMtgStore;
        private static ILoggingService mLoggingService;
        private static BackgroundWorker mUpdaterBackgroundWorker;
        private static List<IPriceUpdater> mPriceUpdaters;
        private static Stopwatch mStopwatch;

        private static void Main(string[] args)
        {
            Bootstrapper.Register();

            mStopwatch = new Stopwatch();
            mLoggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();
            mMtgStore = TinyIoCContainer.Current.Resolve<IMtgStore>();
            mPriceUpdaters = TinyIoCContainer.Current.ResolveAll<IPriceUpdater>().ToList();

            // Setup worker
            mUpdaterBackgroundWorker = new BackgroundWorker();
            mUpdaterBackgroundWorker.WorkerReportsProgress = true;
            mUpdaterBackgroundWorker.WorkerSupportsCancellation = true;
            mUpdaterBackgroundWorker.DoWork += new DoWorkEventHandler(bw_UpdaterDoWork);
            mUpdaterBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_EchoMtgsCompleted);
            mUpdaterBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(bw_EchoMtgProgressChanged);

            try
            {
                Console.WriteLine("Running worker thread for EchoMtg...");
                mUpdaterBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception er)
            {
                string msg = string.Format("Error: {0}", er.Message);
                
                mLoggingService.Fatal(er, msg);
                Console.WriteLine(msg);
            }

            while (mUpdaterBackgroundWorker.IsBusy)
            {
                Thread.Sleep(100);
            }
        }

        private static void bw_UpdaterDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            mStopwatch.Start();

            // Get sets from IMtgStore
            mLoggingService.Debug("Getting sets...");
            List<Set> sets = mMtgStore.GetSets().Result;
            mLoggingService.Debug("Got {0} sets!", sets.Count);

            // Go through each IPriceUpdater and call the UpdatePrices method
            foreach (IPriceUpdater priceUpdater in mPriceUpdaters)
            {
                try
                {
                    foreach (Set set in sets)
                    {
                        priceUpdater.UpdatePrices(set);
                    }
                }
                catch (Exception er)
                {
                    string msg = string.Format("Error in price updater '{0}': {1}",
                        priceUpdater.GetType(),
                        er.Message);

                    mLoggingService.Error(er, msg);
					mLoggingService.Error (er.StackTrace);

                    Console.WriteLine(msg);
					Console.WriteLine (er.StackTrace);
                }
            }
        }

        private static void bw_EchoMtgsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mStopwatch.Stop();

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

            Console.WriteLine("Elapsed time: {0}", mStopwatch.Elapsed);
        }

        private static void bw_EchoMtgProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
    }
}
