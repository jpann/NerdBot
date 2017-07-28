using System.Collections.Generic;
using System.Threading.Tasks;
using NerdBot.Mtg;

namespace NerdBot.Importer
{
    public interface IImporter
    {
        Task<ImportStatus> Import(Set set, IEnumerable<Card> cards);
    }
}