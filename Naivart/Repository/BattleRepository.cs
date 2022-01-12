using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class BattleRepository : Repository<Battle>, IBattleRepository
    {
        public BattleRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<List<Battle>> BattlesAsync(long kingdomId)
        {
            return await Task.FromResult(DbContext.Battles.Where(x => x.AttackerId == kingdomId || x.DefenderId == kingdomId)
                                        .Include(x => x.AttackingTroops)?.Include(x => x.DeadTroops).ToList());
        }
        public async Task<Kingdom> DefenderAsync(long defenderId)
        {
            return await Task.FromResult(DbContext.Kingdoms.Where(x => x.Id == defenderId).Include(x => x.Troops)
                                             .ThenInclude(x => x.TroopType).Include(x => x.Resources).FirstOrDefault());
        }
        public async Task<Kingdom> AttackerAsync(long attackerId)
        {

            return await Task.FromResult(DbContext.Kingdoms.Where(x => x.Id == attackerId).Include(x => x.Troops)
                                               .ThenInclude(x => x.TroopType).Include(x => x.Resources).FirstOrDefault());
        }
        public async Task UpdateTroopsAsync(List<Troop> troops)
        {
            DbContext.UpdateRange(troops);
            await DbContext.SaveChangesAsync();
        }
    }
}

