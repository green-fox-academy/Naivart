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
        public async Task<List<Troop>> GetRecruitingTroops(long kingdomId)
        {
            return await Task.FromResult(DbContext.Troops.Where(x => x.KingdomId == kingdomId && x.Status == "recruiting")
                                                         .ToList());
        }
    }
}
