using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Messengers;
using NerdBot.Parsers;
using NerdBot.Plugin;

namespace NerdBot
{
    public interface IPluginManager
    {
        string PluginDirectory { get; set; }
        List<IPlugin> Plugins { get; }

        void LoadPlugins();
        void UnloadPlugins();
        void SendMessage(IMessage message, IMessenger messenger);
        Task<bool> HandleCommand(Command command, IMessage message, IMessenger messenger);
    }
}
