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
        string ImageUrl { get; set; }
        string ImageHiResUrl { get; set; }

        Card GetCard(TCard source);
        Card GetCard(TCard source, string setName, string setCode);
        Set GetSet(TSet source);
    }
}
