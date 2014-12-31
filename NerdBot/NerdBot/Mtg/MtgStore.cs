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

        public bool CardExists(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            name = name.ToLower();
            setName = setName.ToLower();

            var qty = this.mContext.Cards.Where(c => c.Name.ToLower() == name && c.SetName.ToLower() == setName).Count();

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

        public Card GetCard(string name, string setName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException("setName");

            name = name.ToLower();
            setName = setName.ToLower();

            return
                this.mContext.Cards.Where(
                    c => c.Name.ToLower().StartsWith(name) && 
                        (c.SetName.ToLower().StartsWith(setName)))
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
            var card = this.mContext.Cards.Where(c => c.MultiverseId == multiverseId).FirstOrDefault();
            if (card == null)
                throw new Exception(string.Format("No card exists using multiverse id of '{0}'.", multiverseId));

            var otherSets =
                (
                    from cards in this.mContext.Cards
                    join sets in this.mContext.Sets on cards.SetName equals sets.Name
                    where (cards.MultiverseId != multiverseId && cards.Name == card.Name)
                    select sets
                );

            return otherSets;
        }

        public IEnumerable<Card> GetCardsBySet(string setName)
        {
            if (string.IsNullOrEmpty(setName))
                throw new ArgumentException(setName);

            setName = setName.ToLower();

            var set = this.mContext.Sets.Where(s => s.Name.ToLower() == setName).FirstOrDefault();
            if (set == null)
                throw new Exception(string.Format("No set exists using name '{0}'.", setName));

            var cards = this.mContext.Cards.Where(c => c.SetName.ToLower() == setName);

            return cards;
        }

        public bool SetExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            name = name.ToLower();

            var qty = this.mContext.Sets.Where(s => s.Name.ToLower() == name).Count();

            if (qty <= 0)
                return false;
            else
                return true;
        }

        public bool SetExistsByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("code");

            code = code.ToLower();

            var qty = this.mContext.Sets.Where(s => s.Code.ToLower() == code).Count();

            if (qty <= 0)
                return false;
            else
                return true;
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
