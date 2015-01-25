using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot.Statistics
{
    public interface IQueryStatisticsStore
    {
        Task<bool> InsertCardQueryStat(CardQueryStat stat);
    }
}
