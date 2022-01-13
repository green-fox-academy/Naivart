using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class TroopsLostRepository : Repository<TroopsLost>, ITroopsLostRepository
    {
        public TroopsLostRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<TroopsLost>> LostTroopsAttackerAsync(long battleId)
        {
            return await Task.FromResult(DbContext.TroopsLost.Where(x => x.BattleId == battleId && x.IsAttacker).ToList());
        }
        public async Task<List<TroopsLost>> LostTroopsDefenderAsync(long battleId)
        {
            return await Task.FromResult(DbContext.TroopsLost.Where(x => x.BattleId == battleId && !(x.IsAttacker)).ToList());
        }
    }
}
