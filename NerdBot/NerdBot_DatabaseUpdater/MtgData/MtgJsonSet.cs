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
        public string Block { get; set; }
        public int CommonQty { get; set; }
        public int UncommonQty { get; set; }
        public int RareQty { get; set; }
        public int MythicQty { get; set; }
        public int BasicLandQty { get; set; }
    }
}
