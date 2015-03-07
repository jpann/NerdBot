using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NerdBot.Mtg;
using NerdBot.Utilities;
using Nini.Config;
using SimpleLogging.Core;
using SimpleLogging.NLog;
using TinyIoC;

namespace NerdBot_DatabaseUpdater
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            // Get configuration data
            string configFile = Path.Combine(Environment.CurrentDirectory, "DatabaseUpdater.ini");
            if (!File.Exists(configFile))
                throw new Exception("Configuration file 'DatabaseUpdater.ini' does not exist.");

            string dbConnectionString = null;
            string mtgDbName = null;

            LoadConfiguration(configFile, out dbConnectionString, out mtgDbName);

            // Register the instance of ILoggingService
            TinyIoCContainer.Current.Register<ILoggingService>((c, p) => new NLogLoggingService());

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(
                dbConnectionString,
                mtgDbName,
                TinyIoCContainer.Current.Resolve<ILoggingService>(),
                TinyIoCContainer.Current.Resolve<SearchUtility>());
            TinyIoCContainer.Current.Register<IMtgStore>(mtgStore);
        }

        private static void LoadConfiguration(
            string fileName,
            out string dbConnectionString,
            out string mtgDbName)
        {
            IConfigSource source = new IniConfigSource(fileName);

            dbConnectionString = source.Configs["Database"].Get("connectionString");
            if (string.IsNullOrEmpty(dbConnectionString))
                throw new Exception("Configuration file is missing 'connectionString' setting in section 'Database'.");

            mtgDbName = source.Configs["Database"].Get("mtgDbName");
            if (string.IsNullOrEmpty(mtgDbName))
                throw new Exception("Configuration file is missing 'mtgDbName' setting in section 'Database'.");
        }
    }
}
