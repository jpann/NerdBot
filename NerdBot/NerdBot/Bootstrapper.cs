using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.Json;
using Nancy.TinyIoc;
using NerdBot.Admin;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBot.Reporters;
using NerdBotCommon.Autocomplete;
using NerdBotCommon.Http;
using NerdBotCommon.Importer;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Importer.MtgData;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.ThirdParty.ScryFall;
using NerdBotCommon.UrlShortners;
using NerdBotCommon.Utilities;
using Nini.Config;
using SimpleLogging.Core;
using SimpleLogging.NLog;

namespace NerdBot
{
    using Nancy;

    public class SelfHostRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Environment.CurrentDirectory;
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = NerdBotCommon.Properties.Settings.Default.DiagnosticsPassword }; }
        }

        // Use custom IRootPathProvider to prevent multiple root path provider exception
        protected override IRootPathProvider RootPathProvider
        {
            get { return new SelfHostRootPathProvider(); }
        }

        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "/admin/login",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Get configuration data
            string configFile = Path.Combine(Environment.CurrentDirectory, "NerdBot.ini");
            if (!File.Exists(configFile))
                throw new Exception("Configuration file 'NerdBot.ini' does not exist.");

            string dbConnectionString;
            string dbName;
            string priceDbName;
            string[] msgrBotId;
            string msgrBotName;
            string msgrEndPointUrl;
            string[] msgrIgnoreNames;
            string[] botRouteToken;
            string urlUser;
            string urlKey;
            string reporterBotId;
            string reporterBotName;
            string hostUrl;
            string adminUser;
            string adminPassword;
            string imgUrl = null;
            string imgHiResUrl = null;

            this.LoadConfiguration(
                configFile,
                out dbConnectionString,
                out dbName,
                out priceDbName,
                out msgrBotId,
                out msgrBotName,
                out msgrEndPointUrl,
                out msgrIgnoreNames,
                out botRouteToken,
                out urlUser,
                out urlKey,
                out reporterBotId,
                out reporterBotName,
                out hostUrl,
                out adminUser,
                out adminPassword,
                out imgUrl, 
                out imgHiResUrl
                );

            // Register the instance of ILoggingService
            container.Register<ILoggingService>((c, p) => new NLogLoggingService());

            // Register the instance of IUrlShortener
            var bitlyUrl = new BitlyUrlShortener(
                urlUser,
                urlKey);
            container.Register<IUrlShortener>(bitlyUrl);

            // Register the instance of IQueryStatisticsStore
            var queryStatStore = new QueryStatisticsStore(
                dbConnectionString,
                dbName,
                container.Resolve<ILoggingService>()
            );
            container.Register<IQueryStatisticsStore>(queryStatStore);

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(
                dbConnectionString,
                dbName,
                container.Resolve<IQueryStatisticsStore>(),
                container.Resolve<ILoggingService>(),
                container.Resolve<SearchUtility>());
            container.Register<IMtgStore, MtgStore>(mtgStore);

            // Register the instance of IHttpClient
            container.Register<IHttpClient, SimpleHttpClient>();

            // Register the instance of ICommandParser
            container.Register<ICommandParser, CommandParser>();

            // Register IMessenger with names for each BotID.
            List<BotRoute> botRoutes = new List<BotRoute>();

            for (int i = 0; i < msgrBotId.Length; i++)
            {
                var token = botRouteToken[i];

                // Register the new bot instance of IMessenger using the botID as the registration name
                var groupMeMessenger = new GroupMeMessenger(
                    msgrBotId[i],
                    msgrBotName,
                    msgrIgnoreNames,
                    msgrEndPointUrl,
                    container.Resolve<IHttpClient>(),
                    container.Resolve<ILoggingService>());

                container.Register<IMessenger, GroupMeMessenger>(groupMeMessenger, token);

                botRoutes.Add(new BotRoute()
                {
                    SecretToken = token,
                    BotId = msgrBotId[i]
                });
            }

            // Register IReporter
            var reporterMessenger = new GroupMeMessenger(
                reporterBotId,
                reporterBotName,
                msgrIgnoreNames,
                msgrEndPointUrl,
                container.Resolve<IHttpClient>(),
                container.Resolve<ILoggingService>());

            var groupmeReporter = new GroupMeReporter(reporterMessenger);
            container.Register<IReporter>(groupmeReporter);

            // Register the instance of ICardPriceStore
            var priceStore = new EchoMtgPriceStore(
                dbConnectionString,
                priceDbName,
                container.Resolve<ILoggingService>(),
                container.Resolve<SearchUtility>());
            container.Register<ICardPriceStore>(priceStore);

            // Register IAutocompleter
            container.Register<IAutocompleter, ScryFallAutocomplete>();

            // Register the instance of IBotServices
            container.Register<IBotServices, BotServices>();

            // Register BotConfig
            string[] secretToken = botRouteToken;
            var botConfig = new BotConfig()
            {
                BotName = msgrBotName,
                HostUrl = hostUrl,
                AdminUser = adminUser,
                AdminPassword =  adminPassword,
                BotRoutes = botRoutes
            };
            container.Register(botConfig);

            // Register the instance of IPluginManager
            //string pluginDirectory = Environment.CurrentDirectory;
            string pluginDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "plugins");

            var pluginManager = new PluginManager(
                msgrBotName,
                pluginDirectory,
                container.Resolve<ILoggingService>(), 
                container.Resolve<IMtgStore>(), 
                container.Resolve<ICommandParser>(), 
                container.Resolve<IReporter>(),
                container);

            container.Register<IPluginManager>(pluginManager);

            container.Register<IUserMapper, UserMapper>();

            // Register the instance of MtgJsonMapper
            var mtgJsonMapper = new MtgJsonMapper(
                container.Resolve<SearchUtility>());

            mtgJsonMapper.ImageUrl = imgUrl;
            mtgJsonMapper.ImageHiResUrl = imgHiResUrl;

            container.Register<IMtgDataMapper<MtgJsonCard, MtgJsonSet>>(mtgJsonMapper);

            container.Register<IImporter, MtgImporter>();
        }

        private void LoadConfiguration(
            string fileName,
            out string dbConnectionString,
            out string dbName,
            out string priceDbName,
            out string[] msgrBotId,
            out string msgrBotName,
            out string msgrEndPointUrl,
            out string[] msgrIgnoreNames,
            out string[] botRouteToken,
            out string urlUser,
            out string urlKey,
            out string reporterBotId,
            out string reporterBotName,
            out string hostUrl,
            out string adminUser,
            out string adminPassword,
            out string imgUrl,
            out string imgHiResUrl)
        {
            IConfigSource source = new IniConfigSource(fileName);

            dbConnectionString = source.Configs["Database"].Get("connectionString");
            if (string.IsNullOrEmpty(dbConnectionString))
                throw new Exception("Configuration file is missing 'connectionString' setting in section 'Database'.");

            dbName = source.Configs["Database"].Get("dbName");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("Configuration file is missing 'dbName' setting in section 'Database'.");

            priceDbName = source.Configs["Database"].Get("priceDbName");
            if (string.IsNullOrEmpty(priceDbName))
                throw new Exception("Configuration file is missing 'priceDbName' setting in section 'Database'.");

            msgrBotId = source.Configs["Messenger"].Get("botId").Split('|');
            if (msgrBotId.Length < 1)
                throw new Exception("Configuration file is missing 'botId' setting in section 'Messenger'.");

            msgrBotName = source.Configs["Messenger"].Get("botName");
            if (string.IsNullOrEmpty(msgrBotName))
                throw new Exception("Configuration file is missing 'botName' setting in section 'Messenger'.");

            msgrEndPointUrl = source.Configs["Messenger"].Get("endPointUrl");
            if (string.IsNullOrEmpty(msgrEndPointUrl))
                throw new Exception("Configuration file is missing 'endPointUrl' setting in section 'Messenger'.");

            string ignoreNames = source.Configs["Messenger"].Get("ignoreNames");
            msgrIgnoreNames = new string[] { };
            if (!string.IsNullOrEmpty(ignoreNames))
            {
                if (ignoreNames.Contains("|"))
                    msgrIgnoreNames = ignoreNames.Split('|').ToArray();
            }

            botRouteToken = source.Configs["Bot"].Get("routeToken").Split('|');
            if (botRouteToken.Length < 1)
                throw new Exception("Configuration file is missing 'routeToken' setting in section 'Bot'.");

            if (botRouteToken.Length < msgrBotId.Length)
                throw new Exception("Configuration file 'routeToken' setting in section 'Messenger' should have the same number of elements as 'botId', separated by '|'.");

            urlUser = source.Configs["UrlShorten"].Get("user");
            if (string.IsNullOrEmpty(urlUser))
                throw new Exception("Configuration file is missing 'user' setting in section 'UrlShorten'.");

            urlKey = source.Configs["UrlShorten"].Get("key");
            if (string.IsNullOrEmpty(urlKey))
                throw new Exception("Configuration file is missing 'user' setting in section 'UrlShorten'.");

            reporterBotId = source.Configs["Reporter"].Get("botId");
            if (string.IsNullOrEmpty(reporterBotId))
                throw new Exception("Configuration file is missing 'botId' setting in section 'Reporter'.");

            reporterBotName = source.Configs["Reporter"].Get("botName");
            if (string.IsNullOrEmpty(reporterBotName))
                throw new Exception("Configuration file is missing 'botName' setting in section 'Reporter'.");

            hostUrl = source.Configs["App"].Get("url");
            if (string.IsNullOrEmpty(hostUrl))
                throw new Exception("Configuration file is missing 'url' setting in section 'App'.");

            adminUser = source.Configs["Admin"].Get("username");
            if (string.IsNullOrEmpty(adminUser))
                throw new Exception("Configuration file is missing 'username' setting in section 'Admin'.");

            adminPassword = source.Configs["Admin"].Get("password");
            if (string.IsNullOrEmpty(adminPassword))
                throw new Exception("Configuration file is missing 'username' setting in section 'Admin'.");

            imgUrl = source.Configs["DataMapper"].Get("img_url");
            if (string.IsNullOrEmpty(imgUrl))
                throw new Exception("Configuration file is missing 'img_url' setting in section 'DataMapper'.");

            imgHiResUrl = source.Configs["DataMapper"].Get("imghires_url");
            if (string.IsNullOrEmpty(imgHiResUrl))
                throw new Exception("Configuration file is missing 'imghires_url' setting in section 'DataMapper'.");
        }
    }
}