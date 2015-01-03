using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdBot.Messengers;
using NerdBot.Mtg;

namespace NerdBot.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        void Load(IMtgStore store);
        void Unload();
        bool OnMessage(IMessage message, IMessenger messenger);
    }
}
