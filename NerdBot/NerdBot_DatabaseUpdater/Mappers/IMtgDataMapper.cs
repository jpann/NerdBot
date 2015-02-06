using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBot.Mtg;

namespace NerdBot_DatabaseUpdater.Mappers
{
    public interface IMtgDataMapper<TCard, TSet>
    {
        Card GetCard(TCard source);
        Set GetSet(TSet source);
    }
}
