using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot.Mtg.Prices
{
    public interface ICardPriceStore
    {
        string GetSearchValue(string text, bool forRegex = false);

        CardPrice GetCardPrice(string name);
        CardPrice GetCardPrice(string name, string setCode);
        bool RemoveCardPrice(CardPrice cardPrice);
        int RemoveCardPricesOnOrBefore(DateTime date);
        CardPrice FindAndModifyCardPrice(CardPrice cardPrice, bool upsert = true);
    }
}
