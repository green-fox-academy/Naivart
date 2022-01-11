using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Repository
{
    public class BattleRepository : Repository<Battle>, IBattleRepository
    {
        public BattleRepository(ApplicationDbContext context) : base(context)
        {

        }
        public List<Battle> Battles(long kingdomId)
        {
            return DbContext.Battles.Where(x => x.AttackerId == kingdomId || x.DefenderId == kingdomId)
                                        .Include(x => x.AttackingTroops)?.Include(x => x.DeadTroops).ToList();
        }
        public Kingdom Defender(long defenderId)
        {
            return DbContext.Kingdoms.Where(x => x.Id == defenderId).Include(x => x.Troops)
                                             .ThenInclude(x => x.TroopType).Include(x => x.Resources).FirstOrDefault();
        }
        public Kingdom Attacker(long attackerId)
        {

            return DbContext.Kingdoms.Where(x => x.Id == attackerId).Include(x => x.Troops)
                                               .ThenInclude(x => x.TroopType).Include(x => x.Resources).FirstOrDefault();
        }
        public  void UpdateTroops(List<Troop> troops)
        {
            DbContext.UpdateRange(troops);
        }
    }
}

