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

            name = name.ToLower();

            var qty = this.mContext.Cards.Where(c => c.Name.ToLower() == name).Count();

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

            name = name.ToLower();
            set = set.ToLower();

            var qty = this.mContext.Cards.Where(c => c.Name.ToLower() == name && c.SetName.ToLower() == set).Count();

            if (qty <= 0)
                return false;
            else
                return true;
        }

        public Card GetCard(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = name.ToLower();

            return this.mContext.Cards.Where(c => c.Name.ToLower().StartsWith(name)).FirstOrDefault();
        }

        public Card GetCard(string name, string set)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(set))
                throw new ArgumentException("set");

            name = name.ToLower();
            set = set.ToLower();

            return
                this.mContext.Cards.Where(
                    c => c.Name.ToLower().StartsWith(name) && 
                        (c.SetName.ToLower().StartsWith(set)) || c.SetId.ToLower().StartsWith(set))
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

            name = name.ToLower();

            return this.mContext.Cards.Where(c => c.Name.ToLower().StartsWith(name));
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
