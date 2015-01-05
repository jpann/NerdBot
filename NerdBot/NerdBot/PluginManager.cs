using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NerdBot.Extensions;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;
using NerdBot.Plugin;
using Ninject.Extensions.Logging;

namespace NerdBot
{
    public class PluginManager : IPluginManager
    {
        private readonly ILogger mLogger;
        private readonly IMtgStore mStore;
        private readonly ICommandParser mCommandParser;
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

        public PluginManager(ILogger logger, IMtgStore store, ICommandParser commandParser)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;
        }

        public PluginManager(
            string pluginDirectory,
            ILogger logger,
            IMtgStore store,
            ICommandParser commandParser)
        {
            if (string.IsNullOrEmpty(pluginDirectory))
                throw new ArgumentException("pluginDirectory");

            if (!Directory.Exists(pluginDirectory))
                throw new DirectoryNotFoundException(pluginDirectory);

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            this.mPluginDirectory = pluginDirectory;
            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;

            this.LoadPlugins();
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

                    IPlugin plugin = (IPlugin) Activator.CreateInstance(type);

                    plugin.Load(this.mStore, this.mCommandParser);

                    this.mPlugins.Add(plugin);
                }
            }
        }

        public void SendMessage(IMessage message, IMessenger messenger)
        {
            foreach (IPlugin plugin in this.mPlugins)
            {
                bool handled = plugin.OnMessage(message, messenger);
            }
        }
    }
}
