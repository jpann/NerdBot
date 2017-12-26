using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using SimpleLogging.Core;
using TinyIoC;
using Fclp;
using NerdBotCommon.Importer;
using NerdBotCommon.Importer.DataReaders;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Importer.MtgData;
using NerdBotCommon.Mtg;

namespace NerdBot_DatabaseUpdater
{
    class Program
    {
        private static IMtgStore mMtgStore;
        private static ILoggingService mLoggingService;
        private static BackgroundWorker mUpdaterBackgroundWorker;
        private static Stopwatch mStopwatch;
        private static IMtgDataReader mDataReader;
        private static IFileSystem mFileSystem;
        private static IImporter mImporter;
        private static bool mSkipPromos = true;

        internal class WorkArgs
        {
            internal bool ImportByFile { get; set; }
            internal string InputName { get; set; }
        }

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
            bool skipTokens = true;
            string mtgJsonDirectory = null;
            bool importByFile = true;

            try
            {
                mStopwatch = Stopwatch.StartNew();
                mLoggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();
                mMtgStore = TinyIoCContainer.Current.Resolve<IMtgStore>();
                mFileSystem = TinyIoCContainer.Current.Resolve<IFileSystem>();
                mImporter = TinyIoCContainer.Current.Resolve<IImporter>();

                p.Setup<string>("set")
                        .Callback(value => mtgDbInfoSet = value)
                        .Required();

                p.Setup<bool>("mtgjson")
                   .Callback(value => isMtgJson = value)
                   .SetDefault(true);

                p.Setup<string>("file")
                    .Callback(value => mtgJsonFilename = value);

                p.Setup<string>("dir")
                    .Callback(value => mtgJsonDirectory = value);

                p.Setup<bool>("skiptokens")
                   .Callback(value => skipTokens = value)
                   .SetDefault(true);

                p.Setup<bool>("skippromos")
                    .Callback(value => mSkipPromos = value)
                    .SetDefault(true);

                p.Parse(args);

                // Make sure we have either --file or --dir
                if (string.IsNullOrEmpty(mtgJsonDirectory) && string.IsNullOrEmpty(mtgJsonFilename))
                    throw new Exception("You must either use --file or --dir.");

                if (!string.IsNullOrEmpty(mtgJsonDirectory))
                    importByFile = false;

                if (isMtgJson && importByFile)
                {
                    inputName = mtgJsonFilename;
                }
                else if (isMtgJson && !importByFile)
                {
                    inputName = mtgJsonDirectory;

                    if (!Directory.Exists(inputName))
                        throw new DirectoryNotFoundException(inputName);
                }
                else
                {
                    throw new Exception("Please provide either --mtgdbinfo or --mtgjson arguments.");
                }

                mDataReader = new MtgJsonReader(
                        TinyIoCContainer.Current.Resolve<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>("MtgJson"),
                        mLoggingService);

                mDataReader.SkipTokens = skipTokens;

                // Setup worker
                mUpdaterBackgroundWorker = new BackgroundWorker();
                mUpdaterBackgroundWorker.WorkerReportsProgress = true;
                mUpdaterBackgroundWorker.WorkerSupportsCancellation = true;
                mUpdaterBackgroundWorker.DoWork += new DoWorkEventHandler(bw_UpdaterDoWork);
                mUpdaterBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Completed);
                mUpdaterBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            
                Console.WriteLine("Running worker thread...");
                mUpdaterBackgroundWorker.RunWorkerAsync(new WorkArgs { ImportByFile = importByFile, InputName = inputName });
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
            WorkArgs args = e.Argument as WorkArgs;

            string inputName = args.InputName;
            bool importByFile = args.ImportByFile;

            if (importByFile)
            {
                ImportFile(inputName);
            }
            else
            {
                string[] files = Directory.GetFiles(inputName, "*.json");

                foreach (string file in files)
                {
                    ImportFile(file);
                }

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

        private static void ImportFile(string file)
        {
            if (!mFileSystem.File.Exists(file))
                throw new FileNotFoundException(file);

            mDataReader.FileName = file;

            mLoggingService.Debug("Reading set from '{0}'...", file);
            var setData = mFileSystem.File.ReadAllText(file);

            var set = mDataReader.ReadSet(setData);

            if (set == null)
                throw new Exception("Set not found! Aborting.");

            if (set.Type.ToLower() == "promo" && mSkipPromos)
            {
                mLoggingService.Info("Set '{0}' is a promp; skipping...", set.Name);

                return;
            }

            mLoggingService.Debug("Inserting set '{0}' [{1}]...",
                set.Name,
                set.Code);

            var cards = mDataReader.ReadCards(setData);

            var status = mImporter.Import(set, cards).Result;

            if (status != null)
            {
                mLoggingService.Info("Import Successful. Set={0}; CardsImported={1}; CardsFailed={2}",
                    status.ImportedSet.Name,
                    status.ImportedCards.Count,
                    status.FailedCards.Count);
            }
            else
            {
                mLoggingService.Error("Import failed");
            }

            mLoggingService.Debug("Done reading cards from set '{0}'!", file);
        }
    }
}
