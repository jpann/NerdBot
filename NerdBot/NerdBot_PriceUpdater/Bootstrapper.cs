﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.Utilities;
using NerdBot_PriceUpdater.PriceUpdaters;
using Nini.Config;
using SimpleLogging.Core;
using SimpleLogging.NLog;
using TinyIoC;

namespace NerdBot_PriceUpdater
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            string dbConnectionString = null;
            string mtgDbName = null;
            string priceDbName = null;

            if (!RuntimeUtility.IsRunningOnMono())
            {
                // Get configuration data
                string configFile = Path.Combine(Environment.CurrentDirectory, "NerdBotPriceUpdater.ini");
                if (!File.Exists(configFile))
                    throw new Exception("Configuration file 'NerdBotPriceUpdater.ini' does not exist.");

                LoadConfiguration(configFile, out dbConnectionString, out mtgDbName, out priceDbName);
            }
            else
            {
                LoadConfigurationFromEnvironmentVariables(out dbConnectionString, out mtgDbName, out priceDbName);
            }

            // Register the instance of ILoggingService
            TinyIoCContainer.Current.Register<ILoggingService>((c, p) => new NLogLoggingService());

            // Register the instance of IHttpClient
             TinyIoCContainer.Current.Register<IHttpClient, SimpleHttpClient>();

            // Register the instance of IQueryStatisticsStore
            var queryStatStore = new QueryStatisticsStore(
                dbConnectionString,
                mtgDbName,
                TinyIoCContainer.Current.Resolve<ILoggingService>()
            );
            TinyIoCContainer.Current.Register<IQueryStatisticsStore>(queryStatStore);

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(
                dbConnectionString,
                mtgDbName,
                TinyIoCContainer.Current.Resolve<IQueryStatisticsStore>(),
                TinyIoCContainer.Current.Resolve<ILoggingService>(),
                TinyIoCContainer.Current.Resolve<SearchUtility>());
            TinyIoCContainer.Current.Register<IMtgStore>(mtgStore);

            // Register the instance of ICardPriceStore
            var priceStore = new EchoMtgPriceStore(
                dbConnectionString,
                priceDbName,
                TinyIoCContainer.Current.Resolve<ILoggingService>(),
                TinyIoCContainer.Current.Resolve<SearchUtility>());
            TinyIoCContainer.Current.Register<ICardPriceStore>(priceStore);

            // Register the instance of IPriceUpdater
            var priceUpdater = new EchoMtgPriceUpdater(
                TinyIoCContainer.Current.Resolve<IMtgStore>(),
                TinyIoCContainer.Current.Resolve<ICardPriceStore>(),
                TinyIoCContainer.Current.Resolve<IHttpClient>(),
                TinyIoCContainer.Current.Resolve<ILoggingService>(),
                TinyIoCContainer.Current.Resolve<SearchUtility>());
            TinyIoCContainer.Current.Register<IPriceUpdater>(priceUpdater);
        }

        private static void LoadConfiguration(
            string fileName,
            out string dbConnectionString,
            out string mtgDbName,
            out string priceDbName)
        {
            IConfigSource source = new IniConfigSource(fileName);

            dbConnectionString = source.Configs["Database"].Get("connectionString");
            if (string.IsNullOrEmpty(dbConnectionString))
                throw new Exception("Configuration file is missing 'connectionString' setting in section 'Database'.");

            mtgDbName = source.Configs["Database"].Get("mtgDbName");
            if (string.IsNullOrEmpty(mtgDbName))
                throw new Exception("Configuration file is missing 'mtgDbName' setting in section 'Database'.");

            priceDbName = source.Configs["Database"].Get("priceDbName");
            if (string.IsNullOrEmpty(priceDbName))
                throw new Exception("Configuration file is missing 'priceDbName' setting in section 'Database'.");
        }

        private static void LoadConfigurationFromEnvironmentVariables(
            out string dbConnectionString,
            out string mtgDbName,
            out string priceDbName)
        {
            string dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            dbConnectionString = "mongodb://" + dbServer;

            mtgDbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "mtgdb";
            priceDbName = mtgDbName;
        }
    }
}
