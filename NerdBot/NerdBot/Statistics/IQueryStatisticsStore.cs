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
        Task<CardQueryStatData> GetCardQueryStatByMultiverseId(int multiverseId);
        Task<CardQueryStatData> GetCardQueryStatByUserId(int userId);
        Task<CardQueryStatData> GetCardQueryStatByUserName(string userName);
    }
}
