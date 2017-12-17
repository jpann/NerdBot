using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;

namespace NerdBot.Plugin
{
    public interface IPlugin
    {
        IBotServices Services { get; set; }
        BotConfig Config { get; set; }

        string Name { get; }
        string Description { get; }
        string ShortDescription { get; }
        string Command { get; }
        string HelpCommand { get; }
        string HelpDescription { get; }

        void OnLoad();
        void OnUnload();
        Task<bool> OnMessage(IMessage message, IMessenger messenger);
        Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger);
    }
}
