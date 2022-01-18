using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface ITroopTypeRepository : IRepository<TroopType>
    {
        Task<TroopType> GetTroopTypeForUpgradeAsync(string troopType, long troopTypeLevel);
        Task<TroopType> UpgradeStatsOfTroopAsync(string type, long troopTypeLevel);
        Task<int> TotalDamageAsync(AttackerTroops troops);
        Task<bool> DoesTypeExist(string type);
    }
}
