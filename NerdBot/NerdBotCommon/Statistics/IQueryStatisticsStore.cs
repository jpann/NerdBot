using System.Collections.Generic;
using System.Threading.Tasks;

namespace NerdBotCommon.Statistics
{
    public interface IQueryStatisticsStore
    {
        Task<bool> InsertCardQueryStat(string userName, string userId, int multiverseId, string searchTerm);
        Task<List<CardQueryStat>> GetCardQueryStatsByMultiverseId(int multiverseId);
        Task<List<CardQueryStat>> GetCardQueryStatsByUserId(string userId);
        Task<List<CardQueryStat>> GetCardQueryStatsByUserName(string userName);

        Task<List<CardQueryStat>> GetCardQueryStatsBySearchTerm(string searchTerm);

        void InsertQueryStat(string searchTerm, int multiverseId);

        Task<List<QueryStat>> GetQueryStatsBySearchTerm(string searchTerm);

        Task<List<QueryStat>> GetQueryStatsByMultiverseId(int multiverseId);
    }
}
