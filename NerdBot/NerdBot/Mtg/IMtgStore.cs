using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    public interface IMtgStore
    {
        bool CardExists(int multiverseId);
        bool CardExists(string name);
        bool CardExists(string name, string setName);

        Card GetCard(string name);
        Card GetCard(string name, string setName);
        Card SearchCard(string name);
        Card SearchCard(string name, string setName);

        IEnumerable<Card> GetCards();
        IEnumerable<Card> GetCards(string name);
        List<Card> SearchCards(string name);
        List<Card> SearchCards(string name, string setName);

        IEnumerable<Set> GetCardOtherSets(int multiverseId);

        IEnumerable<Card> GetCardsBySet(string setName);

        bool SetExists(string name);
        bool SetExistsByCode(string code);

        Set GetSet(string name);
        Set GetSetByCode(string code);
    }
}
