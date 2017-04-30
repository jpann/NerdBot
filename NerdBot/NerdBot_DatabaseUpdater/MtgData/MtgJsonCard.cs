using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot_DatabaseUpdater.MtgData
{
    public class MtgJsonCard
    {
        public string Layout { get; set; } // Contains layout of card, e.g. layout: "token" or layout: "normal"
        public string Name { get; set; }
        public string MultiverseId { get; set; }
        public int CMC { get; set; }
        public string[] Colors { get; set; }
        public string[] Types { get; set; }
        public string[] SubTypes { get; set; }
        public string[] SuperTypes { get; set; }
        public string Rarity { get; set; }
        public string Artist { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string ManaCost { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Loyalty { get; set; }
        public string Number { get; set; }
        public List<MtgJsonRuling> Rulings { get; set; }
    }
}
