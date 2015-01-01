using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdBot.Messengers;
using NerdBot.Plugin;

namespace NerdBot
{
    public interface IPluginManager
    {
        string PluginDirectory { get; set; }
        List<IPlugin> Plugins { get; }

        void LoadPlugins();
        void SendMessage(IMessage message, IMessenger messenger);
    }
}
