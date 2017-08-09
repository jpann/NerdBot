using System.Threading.Tasks;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;

namespace NerdBotCommon.Plugin
{
    public interface IMessagePlugin
    {
        string BotName { get; set; }
        IMtgStore Store { get; set; }
        ICommandParser CommandParser { get; set; }
        IHttpClient HttpClient { get; set; }
        IUrlShortener UrlShortener { get; set; }
        ICardPriceStore PriceStore { get; set; }

        string Name { get; }
        string Description { get; }
        string ShortDescription { get; }

        void OnLoad();
        void OnUnload();
        Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}