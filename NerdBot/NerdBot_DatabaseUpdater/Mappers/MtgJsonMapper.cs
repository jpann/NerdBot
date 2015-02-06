using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Mtg;
using NerdBot_DatabaseUpdater.MtgData;

namespace NerdBot_DatabaseUpdater.Mappers
{
    class MtgJsonMapper : IMtgDataMapper<MtgJsonCard, MtgJsonSet>
    {
        public Card GetCard(MtgJsonCard source)
        {
            throw new NotImplementedException();
        }

        public Set GetSet(MtgJsonSet source)
        {
            throw new NotImplementedException();
        }

    }
}
