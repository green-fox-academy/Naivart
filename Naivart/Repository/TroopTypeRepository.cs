using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class TroopTypeRepository : Repository<TroopType>,ITroopTypeRepository
    {
        public TroopTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<TroopType> GetTroopTypeForUpgradeAsync(string troopType,long troopTypeLevel)
        {
           return await Task.FromResult(DbContext.TroopTypes.Where(x => x.Type == troopType && x.Level == troopTypeLevel).FirstOrDefault());
        }
        public async Task<TroopType> UpgradeStatsOfTroopAsync(string type, long troopTypeLevel)
        {
            return await Task.FromResult(DbContext.TroopTypes.Where(t => t.Type == type && t.Level == troopTypeLevel + 1).FirstOrDefault());
        }

        public async Task<int> TotalDamageAsync(AttackerTroops troops)
        {
            return await Task.FromResult(DbContext.TroopTypes.FirstOrDefault(x => x.Type == troops.Type &&
                                 x.Level == troops.Level).Attack * 6 * troops.Quantity);
        }

        public async Task<bool> DoesTypeExist(string type)
        {
            return await Task.FromResult(DbContext.TroopTypes.Any(x => x.Type == type));
        }
    }
}
