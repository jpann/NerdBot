using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot_DatabaseUpdater.MtgData
{
    public class MtgJsonSet
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Type { get; set; }
        public List<MtgJsonCard> Cards { get; set; }
        public string Block { get; set; }
    }
}
