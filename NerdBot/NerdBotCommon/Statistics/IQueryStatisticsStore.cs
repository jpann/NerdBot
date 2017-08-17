using System.Threading.Tasks;

namespace NerdBotCommon.Statistics
{
    public interface IQueryStatisticsStore
    {
        Task<bool> InsertCardQueryStat(string userName, int userId, int multiverseId);
        Task<CardQueryStatData> GetCardQueryStatByMultiverseId(int multiverseId);
        Task<CardQueryStatData> GetCardQueryStatByUserId(int userId);
        Task<CardQueryStatData> GetCardQueryStatByUserName(string userName);
    }
}
