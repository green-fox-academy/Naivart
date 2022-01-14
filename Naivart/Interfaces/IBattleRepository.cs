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
        Task<bool> DoesBattleExistAsync(long battleId);
        Task<bool> IsKingdomInBattleAsync(long battleId, long kingdomId);
        Task<Battle> GetBattleFromBattleIdAsync(long battleId);
        Task<bool> IsKingdomInBattleAsync(long kingdomId);
        Task<bool> IsAttackerInBattle(long kingdomId);
    }
}
