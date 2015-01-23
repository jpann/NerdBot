using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            // Opps, this is causing 'System.InvalidOperationException: Collection was modified; enumeration operation may not execute.'
            //this.UnloadPlugins();
        }

        public void LoadPlugins()
        {
            this.mLogger.Debug("Loading plugins from {0}...", this.mPluginDirectory);

            DirectoryInfo info = new DirectoryInfo(this.mPluginDirectory);

            foreach (FileInfo fileInfo in info.GetFiles("*.dll"))
            {
                Assembly currentAssembly = Assembly.LoadFile(fileInfo.FullName);

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.ImplementsInterface(typeof(IPlugin)))
                        continue;

                    this.mLogger.Debug("Loading plugin '{0}'...", currentAssembly.FullName);

                    IPlugin plugin = (IPlugin)this.mContainer.Resolve(type);

                    plugin.OnLoad();

                    this.mLogger.Debug("Loaded plugin '{0}'!", currentAssembly.FullName);

                    this.mPlugins.Add(plugin);
                }
            }

            this.mLogger.Debug("Loaded {0} plugins.", this.Plugins.Count);
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
            try
            {
                foreach (IPlugin plugin in this.mPlugins)
                {
                    bool handled = await plugin.OnMessage(message, messenger);
                }
            }
            catch (Exception er)
            {
                string msg = string.Format("Error sending message to plugins: {0}", er.Message);

                Console.WriteLine(msg);
                this.mLogger.Error(er, msg);
            }
        }

        public async Task<bool> HandleCommand(
            Command command, 
            IMessage message, 
            IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            try
            {
                foreach (IPlugin plugin in this.mPlugins)
                {
                    if (plugin.Command == command.Cmd)
                    {
                        this.mLogger.Debug("Calling OnCommand for {0}' in plugin '{1}'...",
                            command.Cmd,
                            plugin.Name);

                        bool handled = await plugin.OnCommand(command, message, messenger);

                        return handled;
                    }
                }
            }
            catch (Exception er)
            {
                string msg = string.Format("Error sending command to plugins: {0}", er.Message);

                Console.WriteLine(msg);
                this.mLogger.Error(er, msg);
            }

            return false;
        }

        public async Task<bool> HandledHelpCommand(Command command, IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            // If no arguments were used, return false because the command wasn't handled
            if (!command.Arguments.Any())
                return false;

            try
            {
                string argument = command.Arguments[0];

                foreach (IPlugin plugin in this.mPlugins)
                {
                    string helpCmd = command.Cmd + " " + argument;

                    if (plugin.HelpCommand.ToLower() == helpCmd)
                    {
                        string helpText = plugin.HelpDescription;

                        messenger.SendMessage(helpText);

                        return true;
                    }
                }

                // Check for core help commands
                // Get list of available commands
                if (argument == "commands")
                {
                    string msg;

                    if (!this.Plugins.Any())
                    {
                        msg = "No commands available.";
                    }
                    else
                    {
                        string availableCommands = string.Join(", ", this.Plugins.Select(p => p.Command).ToArray());

                        msg = string.Format("Available commands: {0}", availableCommands);
                    }

                    messenger.SendMessage(msg);

                    return true;
                }
            }
            catch (Exception er)
            {
                string msg = string.Format("Error handling help command: {0}", er.Message);

                Console.WriteLine(msg);
                this.mLogger.Error(er, msg);
            }

            return false;
        }
    }
}
