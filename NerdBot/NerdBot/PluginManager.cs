using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nancy.TinyIoc;
using NerdBotCommon.Extensions;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.Reporters;
using SimpleLogging.Core;

namespace NerdBotCommon
{
    public class PluginManager : IPluginManager
    {
        private readonly ILoggingService mLogger;
        private readonly IMtgStore mStore;
        private readonly ICommandParser mCommandParser;
        private readonly TinyIoCContainer mContainer;
        private readonly IReporter mReporter;
        private string mPluginDirectory;
        private List<IPlugin> mPlugins = new List<IPlugin>();
        private List<IMessagePlugin> mMessagePlugins = new List<IMessagePlugin>();
        private string mBotName;

        #region Properties
        public string BotName
        {
            set { this.mBotName = value; }
            get { return this.mBotName; }
        }

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

        public List<IMessagePlugin> MessagePlugins
        {
            get { return this.mMessagePlugins; }
        }
        #endregion

        public PluginManager(ILoggingService logger, IMtgStore store, ICommandParser commandParser, IReporter reporter, TinyIoCContainer container)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (reporter == null)
                throw new ArgumentNullException("reporter");

            if (container == null)
                throw new ArgumentNullException("container");

            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;
            this.mReporter = reporter;
            this.mContainer = container;
        }

        public PluginManager(
            string botName,
            string pluginDirectory,
            ILoggingService logger,
            IMtgStore store,
            ICommandParser commandParser,
            IReporter reporter, 
            TinyIoCContainer container)
        {
            if (string.IsNullOrEmpty(botName))
                throw new ArgumentException("botName");

            if (string.IsNullOrEmpty(pluginDirectory))
                throw new ArgumentException("pluginDirectory");

            if (!Directory.Exists(pluginDirectory))
                throw new DirectoryNotFoundException(pluginDirectory);

            if (store == null)
                throw new ArgumentNullException("store");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (reporter == null)
                throw new ArgumentNullException("reporter");

            if (container == null)
                throw new ArgumentNullException("container");

            this.mBotName = botName;
            this.mPluginDirectory = pluginDirectory;
            this.mLogger = logger;
            this.mStore = store;
            this.mCommandParser = commandParser;
            this.mContainer = container;
            this.mReporter = reporter;

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

                    this.mLogger.Debug("Loading plugin '{0}' from '{1}'...", 
                        type.ToString(),
                        currentAssembly.GetName());

                    IPlugin plugin = (IPlugin)this.mContainer.Resolve(type);

                    this.mContainer.BuildUp(plugin);

                    plugin.OnLoad();

                    this.mLogger.Debug("Loaded plugin '{0}'!",
                        type.ToString());

                    this.mPlugins.Add(plugin);
                }
            }

            this.mLogger.Debug("Loaded {0} plugins.", this.Plugins.Count);

            // Load IMessagePlugin plugins
            this.mLogger.Debug("Loading message plugins from {0}...", this.mPluginDirectory);

            DirectoryInfo mpInfo = new DirectoryInfo(this.mPluginDirectory);

            foreach (FileInfo fileInfo in mpInfo.GetFiles("*.dll"))
            {
                Assembly currentAssembly = Assembly.LoadFile(fileInfo.FullName);

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.ImplementsInterface(typeof(IMessagePlugin)))
                        continue;

                    this.mLogger.Debug("Loading message plugin '{0}' from '{1}'...",
                        type.ToString(),
                        currentAssembly.GetName());

                    IMessagePlugin plugin = (IMessagePlugin)this.mContainer.Resolve(type);

                    this.mContainer.BuildUp(plugin);

                    plugin.BotName = this.mBotName;
                    plugin.OnLoad();

                    this.mLogger.Debug("Loaded message plugin '{0}'!",
                        type.ToString());

                    this.mMessagePlugins.Add(plugin);
                }
            }

            this.mLogger.Debug("Loaded {0} message plugins.", this.MessagePlugins.Count);
        }

        public void UnloadPlugins()
        {
            foreach (IPlugin plugin in this.mPlugins)
            {
                plugin.OnUnload();

                this.mPlugins.Remove(plugin);
            }

            foreach (IMessagePlugin plugin in this.mMessagePlugins)
            {
                plugin.OnUnload();

                this.mMessagePlugins.Remove(plugin);
            }
        }

        public async void SendMessage(IMessage message, IMessenger messenger)
        {
            try
            {
                foreach (IMessagePlugin plugin in this.mMessagePlugins)
                {
                    bool handled = await plugin.OnMessage(message, messenger);
                }
            }
            catch (Exception er)
            {
                string msg = string.Format("Error sending message to plugins: {0}", er.Message);

                Console.WriteLine(msg);
                this.mLogger.Error(er, msg);
                this.mReporter.Error(msg, er);
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
                this.mReporter.Error(msg, er);
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
            {
                this.mLogger.Trace("Command '{0}' had no arguments provided.", command.Cmd);

                return false;
            }

            try
            {
                string argument = command.Arguments[0].ToLower().Trim();

                this.mLogger.Debug("Help argument: {0}", argument);

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

                    this.mLogger.Trace("Available commands: {0}", msg);

                    messenger.SendMessage(msg);

                    return true;
                }
            }
            catch (Exception er)
            {
                string msg = string.Format("Error handling help command: {0}", er.Message);

                Console.WriteLine(msg);
                this.mLogger.Error(er, msg);
                this.mReporter.Error(msg, er);
            }

            return false;
        }
    }
}
