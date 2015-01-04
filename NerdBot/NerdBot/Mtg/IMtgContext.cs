using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    public interface IMtgContext
    {
        IDbSet<Card> Cards { get; set; }
        IDbSet<Set> Sets { get; set; }

        int SaveChanges();
        List<T> ExecuteQuery<T>(string query, object[] parameters);
    }
}
