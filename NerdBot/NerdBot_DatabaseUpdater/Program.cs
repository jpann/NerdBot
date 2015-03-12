using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NerdBot_DatabaseUpdater.DataReaders;
using NerdBot_DatabaseUpdater.Mappers;
using SimpleLogging.Core;
using TinyIoC;
using Fclp;
using MtgDb.Info.Driver;
using NerdBot_DatabaseUpdater.MtgData;

namespace NerdBot_DatabaseUpdater
{
    class Program
    {
        private static IMtgStore mMtgStore;
        private static ILoggingService mLoggingService;
        private static BackgroundWorker mUpdaterBackgroundWorker;
        private static Stopwatch mStopwatch;
        private static IMtgDataReader mDataReader;

        static void Main(string[] args)
        {
            Bootstrapper.Register();

            var p = new FluentCommandLineParser();
            string inputName = null;

            // Parse command line arguments
            bool isMtgDbInfo = false;
            string mtgDbInfoSet = null;
            bool isMtgJson = false;
            string mtgJsonFilename = null;

            try
            {
                mStopwatch = new Stopwatch();
                mLoggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();
                mMtgStore = TinyIoCContainer.Current.Resolve<IMtgStore>();

                //TODO Not parsing
                p.Setup<bool>("mtgdbinfo")
                   .Callback(value => isMtgDbInfo = value)
                   .SetDefault(false);

                p.Setup<bool>("mtgjson")
                   .Callback(value => isMtgJson = value)
                   .SetDefault(false);

                if (isMtgJson && isMtgDbInfo)
                    throw new Exception("Can only use --mtgdbinfo or --mtgjson, but not both.");

                if (isMtgDbInfo)
                {
                    p.Setup<string>("set")
                        .Callback(value => mtgDbInfoSet = value)
                        .Required();

                    inputName = mtgDbInfoSet;

                    mDataReader = new MtgInfoReader(
                        mtgDbInfoSet,
                        TinyIoCContainer.Current.Resolve<Db>(),
                        TinyIoCContainer.Current.Resolve<IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet>>(
                            "MtgDbInfo"),
                        mLoggingService);
                }

                if (isMtgJson)
                {
                    p.Setup<string>("file")
                        .Callback(value => mtgJsonFilename = value)
                        .Required();

                    inputName = mtgJsonFilename;

                    mDataReader = new MtgJsonReader(
                        mtgJsonFilename,
                        TinyIoCContainer.Current.Resolve<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>("MtgJson"),
                        TinyIoCContainer.Current.Resolve<IFileSystem>(),
                        mLoggingService);
                }

                // Setup worker
                mUpdaterBackgroundWorker = new BackgroundWorker();
                mUpdaterBackgroundWorker.WorkerReportsProgress = true;
                mUpdaterBackgroundWorker.WorkerSupportsCancellation = true;
                mUpdaterBackgroundWorker.DoWork += new DoWorkEventHandler(bw_UpdaterDoWork);
                mUpdaterBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Completed);
                mUpdaterBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

            
                Console.WriteLine("Running worker thread...");
                mUpdaterBackgroundWorker.RunWorkerAsync(inputName);
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
            string inputName = e.Argument.ToString();

            mLoggingService.Debug("Reading set from '{0}'...", inputName);
            var set = mDataReader.ReadSet();

            if (set == null)
                throw new Exception("Set not found! Aborting.");

            mLoggingService.Debug("Inserting set '{0}' [{1}]...", 
                set.Name,
                set.Code);

            var setInserted = mMtgStore.SetFindAndModify(set).Result;

            if (setInserted == null)
                throw new Exception(string.Format("Set '{0}' not inserted!", set.Name));

            mLoggingService.Debug("Reading cards from set '{0}'...", inputName);
            var cards = mDataReader.ReadCards();

            foreach (Card card in cards)
            {
                mLoggingService.Debug("Read card '{0}'...", card.Name);

                var cardInserted = mMtgStore.CardFindAndModify(card).Result;

                if (cardInserted == null)
                {
                    mLoggingService.Warning("Card '{0}' in set '{1}' not inserted!",
                        card.Name,
                        set.Name);

                    continue;
                }

                mLoggingService.Debug("Inserted card '{0}' in set '{1}'!",
                    cardInserted.Name,
                    set.Name);
            }
        }

        private static void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
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

        private static void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
    }
}
