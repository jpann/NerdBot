using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    public class MtgStore : IMtgStore
    {
        private readonly IMtgContext mContext;

        public MtgStore(IMtgContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.mContext = context;
        }

        public Card GetCard(string name)
        {
            throw new NotImplementedException();
        }

        public Card GetCard(string name, string set)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetCards()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetCards(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            return this.mContext.Cards.Where(c => c.Name.ToLower().StartsWith(name.ToLower()));
        }

        public IEnumerable<Card> GetCards(string name, string set)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Set> GetCardOtherSets(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetCardsBySet(string set)
        {
            throw new NotImplementedException();
        }

        public Set GetSet(string name)
        {
            throw new NotImplementedException();
        }

        public Set GetSetByCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
