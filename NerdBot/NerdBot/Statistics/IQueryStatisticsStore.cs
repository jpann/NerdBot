using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot.Statistics
{
    public interface IQueryStatisticsStore
    {
        Task<bool> InsertCardQueryStat(string userName, int userId, int multiverseId);
        Task<CardQueryStat> GetCardQueryStatByMultiverseId(int multiverseId);
        Task<CardQueryStat> GetCardQueryStatByUserId(int userId);
        Task<CardQueryStat> GetCardQueryStatByUserName(string userName);
    }
}
