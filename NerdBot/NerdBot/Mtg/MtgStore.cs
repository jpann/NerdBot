using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
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

        public bool CardExists(int multiverseId)
        {
            var qty = this.mContext.Cards.Where(c => c.MultiverseId == multiverseId).Count();

            if (qty <= 0)
                return false;
            else
                return true;
        }

        public bool CardExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            var qty = this.mContext.Cards.Where(c => c.Name.ToLower() == name.ToLower()).Count();

            if (qty <= 0)
                return false;
            else
                return true;
        }

        public bool CardExists(string name, string set)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(set))
                throw new ArgumentException("set");

            var qty = this.mContext.Cards.Where(c => c.Name.ToLower() == name.ToLower() && c.SetName.ToLower() == set.ToLower()).Count();

            if (qty <= 0)
                return false;
            else
                return true;
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
            bool exists = this.CardExists(multiverseId);
            if (!exists)
                throw new Exception(string.Format("No card exists using multiverse id of '{0}'.", multiverseId));

            var card = this.mContext.Cards.Where(c => c.MultiverseId == multiverseId).FirstOrDefault();

            var otherSets =
                (
                    from cards in this.mContext.Cards
                    join sets in this.mContext.Sets on cards.SetName equals sets.Name
                    where (cards.MultiverseId != multiverseId && cards.Name == card.Name)
                    select sets
                );

            return otherSets;
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
