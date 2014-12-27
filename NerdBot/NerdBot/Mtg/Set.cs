using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    [Table("Sets")]
    public class Set
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("set_id")]
        public string Code { get; set; }

        [Column("block")]
        public string Block { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("desc")]
        public string Desc { get; set; }

        [Column("common_qty")]
        public int CommonQty { get; set; }

        [Column("uncommon_qty")]
        public int UncommonQty { get; set; }

        [Column("rare_qty")]
        public int RareQty { get; set; }

        [Column("mythic_qty")]
        public int MythicQty { get; set; }

        [Column("basic_land_qty")]
        public int BasicLandQty { get; set; }

        [Column("total_qty")]
        public int TotalQty { get; set; }

        [Column("released_at")]
        public DateTime ReleasedOn { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
