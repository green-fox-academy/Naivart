using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface ITroopRepository : IRepository<Troop>
    {
        Task<List<Troop>> GetRecruitingTroops(long kingdomId);
    }
}
