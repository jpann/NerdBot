using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy.TinyIoc;
using NerdBot.Extensions;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.Plugin;
using SimpleLogging.Core;

namespace NerdBot
{
    public class PluginManager : IPluginManager
    {
        private readonly ILoggingService mLogger;
        private readonly IMtgStore mStore;
        private readonly ICommandParser mCommandParser;
        private readonly TinyIoCContainer mContainer;
        private string mPluginDirectory;
        private List<IPlugin> mPlugins = new List<IPlugin>();

        #region Properties
        public string PluginDirectory
        {
            get { return this.mPluginDirectory; }
            set
            {
                if (!Directory.Exists(value))
                    throw new DirectoryNotFoundException(value);

                this.mPluginDirectory = value;
            }
        }

        public List<IPlugin> Plugins
        {
            get { return this.mPlugins; }
        }
        #endregion

        public PluginManager(ILoggingService logger, IMtgStore store, ICommandParser commandParser, TinyIoCContainer container)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (container == null)
                throw new ArgumentNullException("container");

            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;
            this.mContainer = container;
        }

        public PluginManager(
            string pluginDirectory,
            ILoggingService logger,
            IMtgStore store,
            ICommandParser commandParser,
            TinyIoCContainer container)
        {
            if (string.IsNullOrEmpty(pluginDirectory))
                throw new ArgumentException("pluginDirectory");

            if (!Directory.Exists(pluginDirectory))
                throw new DirectoryNotFoundException(pluginDirectory);

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (container == null)
                throw new ArgumentNullException("container");

            this.mPluginDirectory = pluginDirectory;
            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;
            this.mContainer = container;

            this.LoadPlugins();
        }

        ~PluginManager()
        {
            this.UnloadPlugins();
        }

        public void LoadPlugins()
        {
            DirectoryInfo info = new DirectoryInfo(this.mPluginDirectory);

            foreach (FileInfo fileInfo in info.GetFiles("*.dll"))
            {
                Assembly currentAssembly = Assembly.LoadFile(fileInfo.FullName);

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.ImplementsInterface(typeof(IPlugin)))
                        continue;

                    IPlugin plugin = (IPlugin)this.mContainer.Resolve(type);

                    plugin.OnLoad();

                    this.mPlugins.Add(plugin);
                }
            }
        }

        public void UnloadPlugins()
        {
            foreach (IPlugin plugin in this.mPlugins)
            {
                plugin.OnUnload();

                this.mPlugins.Remove(plugin);
            }
        }

        public async void SendMessage(IMessage message, IMessenger messenger)
        {
            foreach (IPlugin plugin in this.mPlugins)
            {
                bool handled = await plugin.OnMessage(message, messenger);
            }
        }
    }
}
