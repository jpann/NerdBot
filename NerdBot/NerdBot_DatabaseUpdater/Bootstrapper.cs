using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using MtgDb.Info.Driver;
using NerdBot.Mtg;
using NerdBot.Utilities;
using NerdBot_DatabaseUpdater.Mappers;
using NerdBot_DatabaseUpdater.MtgData;
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
            string imgUrl = null;
            string imgHiResUrl = null;

            LoadConfiguration(configFile, out dbConnectionString, out mtgDbName, out imgUrl, out imgHiResUrl);

            // Register the instance of ILoggingService
            TinyIoCContainer.Current.Register<ILoggingService>((c, p) => new NLogLoggingService());

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(
                dbConnectionString,
                mtgDbName,
                TinyIoCContainer.Current.Resolve<ILoggingService>(),
                TinyIoCContainer.Current.Resolve<SearchUtility>());
            TinyIoCContainer.Current.Register<IMtgStore>(mtgStore);

            // Register the instance of MtgInfoMapper
            var mtgInfoMapper = new MtgInfoMapper(
                TinyIoCContainer.Current.Resolve<SearchUtility>());

            TinyIoCContainer.Current.Register<IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet>>(mtgInfoMapper,
                "MtgDbInfo");

            // Register the instance of MtgJsonMapper
            var mtgJsonMapper = new MtgJsonMapper(
                TinyIoCContainer.Current.Resolve<SearchUtility>());

            mtgJsonMapper.ImageUrl = imgUrl;
            mtgJsonMapper.ImageHiResUrl = imgHiResUrl;

            TinyIoCContainer.Current.Register<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>(mtgJsonMapper, 
                "MtgJson");

            // Register the instance of Db
            var mtgInfoDb = new Db();
            TinyIoCContainer.Current.Register<Db>(mtgInfoDb);

            // Register the instance of IFileSystem
            var fileSystem = new FileSystem();
            TinyIoCContainer.Current.Register<IFileSystem>(fileSystem);
        }

        private static void LoadConfiguration(
            string fileName,
            out string dbConnectionString,
            out string mtgDbName,
            out string imgUrl,
            out string imgHiResUrl)
        {
            IConfigSource source = new IniConfigSource(fileName);

            dbConnectionString = source.Configs["Database"].Get("connectionString");
            if (string.IsNullOrEmpty(dbConnectionString))
                throw new Exception("Configuration file is missing 'connectionString' setting in section 'Database'.");

            mtgDbName = source.Configs["Database"].Get("dbName");
            if (string.IsNullOrEmpty(mtgDbName))
                throw new Exception("Configuration file is missing 'dbName' setting in section 'Database'.");

            imgUrl = source.Configs["DataMapper"].Get("img_url");
            if (string.IsNullOrEmpty(imgUrl))
                throw new Exception("Configuration file is missing 'img_url' setting in section 'DataMapper'.");

            imgHiResUrl = source.Configs["DataMapper"].Get("imghires_url");
            if (string.IsNullOrEmpty(imgHiResUrl))
                throw new Exception("Configuration file is missing 'imghires_url' setting in section 'DataMapper'.");
        }
    }
}
