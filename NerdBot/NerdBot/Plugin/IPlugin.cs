using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Messengers;
using NerdBot.Mtg;
using NerdBot.Parsers;

namespace NerdBot.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        void Load(IMtgStore store, ICommandParser commandParser);
        void Unload();
        Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}
