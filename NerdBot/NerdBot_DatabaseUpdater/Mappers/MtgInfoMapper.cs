using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NerdBot.Mtg;

namespace NerdBot_DatabaseUpdater.Mappers
{
    public class MtgInfoMapper : IMtgDataMapper<MtgDb.Info.Card, MtgDb.Info.CardSet>
    {
        public Card GetCard(MtgDb.Info.Card source)
        {
            throw new NotImplementedException();
        }

        public Set GetSet(MtgDb.Info.CardSet source)
        {
            throw new NotImplementedException();
        }
    }
}
