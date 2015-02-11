using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Routing.Trie;

namespace NerdBot.Mtg
{
    public interface IMtgStore
    {
        Task<bool> CardExists(int multiverseId);
        Task<bool> CardExists(string name);
        Task<bool> CardExists(string name, string setName);

        Task<Card> GetCard(string name);
        Task<Card> GetCard(string name, string setName);

        Task<List<Card>> GetCards();
        Task<List<Card>> GetCards(string name);

        Task<Card> GetRandomCardByArtist(string artist);
        Task<Card> GetRandomCardInSet(string setName);
        Task<Card> GetRandomCard();
        Task<Card> GetRandomCardWithStaticAbility(string text);

        Task<List<Set>> GetCardSets(int multiverseId, int limit = 8);

        Task<List<Set>> GetCardOtherSets(int multiverseId);

        Task<List<Card>> GetCardsBySet(string setName);

        Task<bool> SetExists(string name);
        Task<bool> SetExistsByCode(string code);

        Task<Set> GetSet(string name);
        Task<Set> GetSetByCode(string code);
        Task<List<Set>> GetSets();
    }
}
