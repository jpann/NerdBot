using System.Collections.Generic;
using NerdBot.Mtg;

namespace NerdBot.Importer.DataReaders
{
    public interface IMtgDataReader
    {
        Set ReadSet(string data);
        IEnumerable<Card> ReadCards(string data);

        bool SkipTokens { set; }
        string FileName { get; set; }
    }
}