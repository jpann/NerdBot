using System.Collections.Generic;
using NerdBot.Mtg;

namespace NerdBot_DatabaseUpdater.DataReaders
{
    public interface IMtgDataReader
    {
        Set ReadSet();
        IEnumerable<Card> ReadCards();

        bool SkipTokens { set; }
        string FileName { get; set; }
    }
}
