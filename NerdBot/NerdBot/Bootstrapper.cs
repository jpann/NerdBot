using System;
using System.Collections;
using System.IO;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.UrlShortners;
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

        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            string pluginDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");

            // Register the instance of ILoggingService
            var loggingService = new NLogLoggingService();
            container.Register<ILoggingService>((c, p) => loggingService);

            // Register the instance of IUrlShortener
            var bitlyUrl = new BitlyUrlShortener(Properties.Settings.Default.UrlShort_User,
                Properties.Settings.Default.UrlShort_Key);
            container.Register<IUrlShortener>(bitlyUrl);

            // Register the instance of IMtgStore
            var mtgStore = new MtgStore(Properties.Settings.Default.ConnectionString,
                Properties.Settings.Default.ConnectionDb);
            container.Register<IMtgStore>(mtgStore);

            // Register the instance of IHttpClient
            container.Register<IHttpClient, SimpleHttpClient>();

            // Register the instance of ICommandParser
            container.Register<ICommandParser, CommandParser>();

            // Register the instance of IMessenger
            var groupMeMessenger = new GroupMeMessenger(Properties.Settings.Default.BotId,
                Properties.Settings.Default.BotName,
                new string[] { },
                Properties.Settings.Default.EndPointUrl,
                container.Resolve<IHttpClient>(),
                loggingService);
            container.Register<IMessenger>(groupMeMessenger);

            // Register the instance of IPluginManager
            var pluginManager = new PluginManager(pluginDirectory, loggingService, container.Resolve<IMtgStore>(), container.Resolve<ICommandParser>(), container);
            container.Register<IPluginManager>(pluginManager);
        }
    }
}