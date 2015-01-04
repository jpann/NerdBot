using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace NerdBot.Mtg
{
    public class MtgContext : DbContext, IMtgContext
    {
        public MtgContext()
            : base("MtgContext")
        {
        }

        public MtgContext(string connectionString)
            : base(connectionString)
        {

        }

        public IDbSet<Card> Cards { get; set; }
        public IDbSet<Set> Sets { get; set; }

        // Usage:
        //List<Card> cards = this.mContext.ExecuteQuery<Card>("SELECT * FROM Cards WHERE name LIKE @p0",
        //        new string[] { "Spore%" });
        //
        public List<T> ExecuteQuery<T>(string query, object[] parameters)
        {
            List<T> results = new List<T>();

            var dbQuery = base.Database.SqlQuery<T>(query, parameters).ToList();

            foreach (T result in dbQuery)
            {
                results.Add(result);
            }

            return results;

        }
    }
}
