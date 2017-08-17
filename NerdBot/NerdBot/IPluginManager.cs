using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;

namespace NerdBot
{
    public interface IPluginManager
    {
        string BotName { get; set; }
        string PluginDirectory { get; set; }
        List<IPlugin> Plugins { get; }
        List<IMessagePlugin> MessagePlugins { get; }

        void LoadPlugins();
        void UnloadPlugins();

        void SendMessage(IMessage message, IMessenger messenger);
        Task<bool> HandleCommand(Command command, IMessage message, IMessenger messenger);
        Task<bool> HandledHelpCommand(Command command, IMessenger messenger);
    }
}
