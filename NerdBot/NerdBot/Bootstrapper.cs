using System;
using System.Collections;
using System.IO;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using NerdBot.Http;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.Plugin;
using Ninject;
using Ninject.Extensions.Conventions;

namespace NerdBot
{
    using Nancy;

    public class Bootstrapper : NinjectNancyBootstrapper
    {
        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(IKernel existingContainer)
        {
            // Perform registation that should have an application lifetime
            string pluginDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");

            existingContainer.Bind<IMtgStore>()
                .To<MtgStore>()
                .InSingletonScope()
                .WithConstructorArgument("connectionString", Properties.Settings.Default.ConnectionString)
                .WithConstructorArgument("databaseName", Properties.Settings.Default.ConnectionDb);

            existingContainer.Bind<IHttpClient>()
                .To<SimpleHttpClient>();

            existingContainer.Bind<ICommandParser>()
                .To<CommandParser>()
                .InSingletonScope();

            existingContainer.Bind<IMessenger>()
                .To<GroupMeMessenger>()
                .InSingletonScope()
                .WithConstructorArgument("botId", Properties.Settings.Default.BotId)
                .WithConstructorArgument("botName", Properties.Settings.Default.BotName)
                .WithConstructorArgument("ignoreNames", new string[] {})
                .WithConstructorArgument("endPointUrl", Properties.Settings.Default.EndPointUrl);

            existingContainer.Bind(
                x => x.FromAssembliesInPath(pluginDirectory)
                .SelectAllClasses().InheritedFrom<IPlugin>()
                .BindAllInterfaces());

            existingContainer.Bind<IPluginManager>()
                .To<PluginManager>()
                .InSingletonScope()
                .WithConstructorArgument("pluginDirectory", pluginDirectory);
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(IKernel container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }
    }
}