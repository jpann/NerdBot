using System;
using System.Collections.Generic;

namespace NerdBot.Importer.MtgData
{
    public class MtgJsonSetExtended
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
        public List<MtgJsonCard> Cards { get; set; }
    }
}
