using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class TroopRepository : Repository<Troop>, ITroopRepository
    {
        public TroopRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Troop>> GetRecruitingOrUpgradingTroops(long kingdomId)
        {
            return await Task.FromResult(DbContext.Troops.Where(x => x.KingdomId == kingdomId 
                                          && (x.Status == "recruiting" || x.Status == "upgrading"))
                                        .Include(x => x.TroopType).ToList());
        }

        public async Task UpgradeTroop(Troop troop)
        {
            long troopTypeId = 
                await Task.FromResult(DbContext.TroopTypes
                .FirstOrDefault(x => x.Type == troop.TroopType.Type && x.Level == (troop.TroopType.Level + 1)).Id);

            troop.TroopTypeId = troopTypeId;
            await DbContext.SaveChangesAsync();
        }
    }
}
