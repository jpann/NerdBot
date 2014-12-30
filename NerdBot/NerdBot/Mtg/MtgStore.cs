using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Infrastructure.Language;

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
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            return this.mContext.Cards.Where(c => c.Name.ToLower().StartsWith(name.ToLower())).FirstOrDefault();
        }

        public Card GetCard(string name, string set)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(set))
                throw new ArgumentException("set");

            return
                this.mContext.Cards.Where(
                    c => c.Name.ToLower().StartsWith(name.ToLower()) && 
                        (c.SetName.ToLower().StartsWith(set.ToLower())) || c.SetId.ToLower().StartsWith(set.ToLower()))
                    .FirstOrDefault();
        }

        public IEnumerable<Card> GetCards()
        {
            return this.mContext.Cards.ToEnumerable();
        }

        public IEnumerable<Card> GetCards(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            return this.mContext.Cards.Where(c => c.Name.ToLower().StartsWith(name.ToLower()));
        }

        public IEnumerable<Set> GetCardOtherSets(int multiverseId)
        {
            var card = this.mContext.Cards.Where(c => c.MultiverseId == multiverseId).FirstOrDefault();
            if (card == null)
                throw new Exception(string.Format("No card exists using multiverse id of '{0}'.", multiverseId));

            //TODO Query for all sets that the other cards are in
            //var otherSets = this.mContext.Cards
            //    .Where(c =>
            //        c.MultiverseId != multiverseId &&
            //        c.Name == card.Name)
            //    .Select(c => c.SetName)
            //    .ToEnumerable();

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
