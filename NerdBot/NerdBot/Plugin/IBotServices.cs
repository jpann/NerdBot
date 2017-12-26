using NerdBot.Parsers;
using NerdBotCommon.Autocomplete;
using NerdBotCommon.Http;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Statistics;
using NerdBotCommon.UrlShortners;

namespace NerdBot.Plugin
{
    public interface IBotServices
    {
        IMtgStore Store { get; }
        ICardPriceStore PriceStore { get; }
        ICommandParser CommandParser { get; }
        IHttpClient HttpClient { get; }
        IUrlShortener UrlShortener { get; }
        IQueryStatisticsStore QueryStatisticsStore { get; }
        IAutocompleter Autocompleter { get; }
    }
}