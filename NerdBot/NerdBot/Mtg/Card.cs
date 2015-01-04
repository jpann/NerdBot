using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    [Table("Cards")]
    public class Card
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("related_card_id")]
        public int? Related_Card_Id { get; set; }

        [Column("set_number")]
        public int Set_Number { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("search_name")]
        public string Search_Name { get; set; }

        [Column("desc")]
        public string Desc { get; set; }

        [Column("flavor")]
        public string Flavor { get; set; }

        [Column("colors")]
        public string Colors { get; set; }

        [Column("cost")]
        public string Cost { get; set; }

        [Column("cmc")]
        public int Cmc { get; set; }

        [Column("set_name")]
        public string Set_Name { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("subtype")]
        public string SubType { get; set; }

        [Column("power")]
        public string Power { get; set; }

        [Column("toughness")]
        public string Toughness { get; set; }

        [Column("loyalty")]
        public string Loyalty { get; set; }

        [Column("rarity")]
        public string Rarity { get; set; }

        [Column("artist")]
        public string Artist { get; set; }

        [Column("set_id")]
        public string Set_Id { get; set; }

        [Column("token")]
        public bool? Token { get; set; }

        [Column("img")]
        public string Img { get; set; }

        [Column("img_hires")]
        public string Img_Hires { get; set; }

        [Column("multiverse_id")]
        public int Multiverse_Id { get; set; }
    }
}
