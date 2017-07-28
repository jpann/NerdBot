using System;
using System.Collections;
using System.IO;
using System.Linq;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.TinyIoc;
using NerdBot.Admin;
using NerdBot.Http;
using NerdBot.Importer;
using NerdBot.Importer.Mapper;
using NerdBot.Importer.MtgData;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Mtg.Prices;
using NerdBot.Parsers;
using NerdBot.Reporters;
using NerdBot.Statistics;
using NerdBot.UrlShortners;
using NerdBot.Utilities;
using Nini.Config;
using SimpleLogging.Core;
using SimpleLogging.NLog;

namespace NerdBot
{
    using Nancy;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
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
            string msgrBotId;
            string msgrBotName;
            string msgrEndPointUrl;
            string[] msgrIgnoreNames;
            string botRouteToken;
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

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(
                dbConnectionString,
                dbName,
                container.Resolve<ILoggingService>(),
                container.Resolve<SearchUtility>());
            container.Register<IMtgStore>(mtgStore);

            // Register the instance of IQueryStatisticsStore
            var queryStatStore = new QueryStatisticsStore(
                dbConnectionString,
                dbName,
                container.Resolve<ILoggingService>()
                );
            container.Register<IQueryStatisticsStore>(queryStatStore);

            // Register the instance of IHttpClient
            container.Register<IHttpClient, SimpleHttpClient>();

            // Register the instance of ICommandParser
            container.Register<ICommandParser, CommandParser>();

            // Register the main instance of IMessenger
            var groupMeMessenger = new GroupMeMessenger(
                msgrBotId,
                msgrBotName,
                msgrIgnoreNames,
                msgrEndPointUrl,
                container.Resolve<IHttpClient>(),
                container.Resolve<ILoggingService>());

            container.Register<IMessenger>(groupMeMessenger);

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

            // Register BotConfig
            string secretToken = botRouteToken;
            var botConfig = new BotConfig()
            {
                SecretToken = secretToken,
                HostUrl = hostUrl,
                AdminUser = adminUser,
                AdminPassword =  adminPassword
            };
            container.Register(botConfig);

            // Register the instance of IPluginManager
            string pluginDirectory = Environment.CurrentDirectory;

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
            out string msgrBotId,
            out string msgrBotName,
            out string msgrEndPointUrl,
            out string[] msgrIgnoreNames,
            out string botRouteToken,
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

            msgrBotId = source.Configs["Messenger"].Get("botId");
            if (string.IsNullOrEmpty(msgrBotId))
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

            botRouteToken = source.Configs["Bot"].Get("routeToken");
            if (string.IsNullOrEmpty(botRouteToken))
                throw new Exception("Configuration file is missing 'routeToken' setting in section 'Bot'.");

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