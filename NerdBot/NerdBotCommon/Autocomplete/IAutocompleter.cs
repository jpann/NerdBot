using System.Collections.Generic;
using System.Threading.Tasks;

namespace NerdBotCommon.Autocomplete
{
    public interface IAutocompleter
    {
        Task<List<string>> GetAutocompleteAsync(string term);
    }
}