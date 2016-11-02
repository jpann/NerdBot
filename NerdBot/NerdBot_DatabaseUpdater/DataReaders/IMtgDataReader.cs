using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Mtg;

namespace NerdBot_DatabaseUpdater.DataReaders
{
    public interface IMtgDataReader
    {
        Set ReadSet();
        IEnumerable<Card> ReadCards();

        bool SkipTokens { set; }
    }
}
