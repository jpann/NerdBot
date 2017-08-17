using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Parsers;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;

namespace NerdBot.Plugin
{
    public interface IPlugin
    {
        IMtgStore Store { get; set; }
        ICommandParser CommandParser { get; set; }
        IHttpClient HttpClient { get; set; }
        IUrlShortener UrlShortener { get; set; }
        ICardPriceStore PriceStore { get; set; }
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
