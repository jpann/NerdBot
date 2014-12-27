using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    public class MtgContext : DbContext, IMtgContext
    {
        public MtgContext()
        {
        }

        public MtgContext(string connectionString)
            : base(connectionString)
        {

        }

        public IDbSet<Card> Cards { get; set; }
        public IDbSet<Set> Sets { get; set; }
    }
}
