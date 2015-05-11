using System;
using System.Collections.Generic;

namespace NerdBot.Mtg.Prices
{
    public interface ICardPriceStore
    {
        CardPrice GetCardPrice(string name);
        CardPrice GetCardPrice(string name, string setCode);
        bool RemoveCardPrice(CardPrice cardPrice);
        int RemoveCardPricesOnOrBefore(DateTime date);
        CardPrice FindAndModifyCardPrice(CardPrice cardPrice, bool upsert = true);
        List<CardPrice> GetCardsByPriceIncrease(int limit = 10);
        List<CardPrice> GetCardsByPriceDecrease(int limit = 10);
    }
}
