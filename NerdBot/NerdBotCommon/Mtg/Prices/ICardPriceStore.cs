using System;
using System.Collections.Generic;

namespace NerdBotCommon.Mtg.Prices
{
    public interface ICardPriceStore
    {
        // Sets
        SetPrice GetSetPriceByCode(string code);
        SetPrice GetSetPrice(string name);
        bool RemoveSetPrice(SetPrice setPrice);
        int RemoveSetPricesOnOrBefore(DateTime date);
        SetPrice FindAndModifySetPrice(SetPrice setPrice, bool upsert = true);

        // Cards
        CardPrice GetCardPrice(int multiverseId);
        CardPrice GetCardPrice(string name);
        CardPrice GetCardPrice(string name, string setCode);
        bool RemoveCardPrice(CardPrice cardPrice);
        int RemoveCardPricesOnOrBefore(DateTime date);
        CardPrice FindAndModifyCardPrice(CardPrice cardPrice, bool upsert = true);
        List<CardPrice> GetCardsByPriceIncrease(int limit = 10);
        List<CardPrice> GetCardsByPriceDecrease(int limit = 10);
    }
}
