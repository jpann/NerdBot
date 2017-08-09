using System.Collections.Generic;
using System.Threading.Tasks;
using NerdBotCommon.Mtg;

namespace NerdBotCommon.Importer
{
    public interface IImporter
    {
        Task<ImportStatus> Import(Set set, IEnumerable<Card> cards);
    }
}