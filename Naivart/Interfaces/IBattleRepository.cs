using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IBattleRepository: IRepository<Battle>
    {
        Task<List<Battle>> BattlesAsync(long kingdomId);
        Task<Kingdom> DefenderAsync(long defenderId);
        Task<Kingdom> AttackerAsync(long attackerId);
        Task UpdateTroopsAsync(List<Troop> troops);
    }
}
