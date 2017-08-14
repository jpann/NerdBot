using System.Collections.Generic;
using NerdBotCommon.Mtg;

namespace NerdBotCommon.Importer.DataReaders
{
    public interface IMtgDataReader
    {
        Set ReadSet(string data);
        IEnumerable<Card> ReadCards(string data);

        bool SkipTokens { set; }
        bool SkipPromos { set; }
        string FileName { get; set; }
    }
}