using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface ITroopRepository : IRepository<Troop>
    {
        Task<List<Troop>> GetRecruitingOrUpgradingTroops(long kingdomId);
        Task UpgradeTroop(Troop troop);
    }
}
