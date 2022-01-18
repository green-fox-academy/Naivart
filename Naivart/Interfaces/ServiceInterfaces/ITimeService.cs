using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    interface ITimeService
    {
        long GetUnixTimeNow();
        Task UpdateAllAsync(string username);
        Task UpdateBuildings(long kingdomId);
        Task UpdateTroopsAsync(long kingdomId);
        Task UpdateResourcesAsync(long kingdomId);
        int CalculateAmount(long lastUpdate, int generation, out int extra);
        Task UpdateBattleAsync(long kingdomId);
        List<TroopBattleInfo> GetTroopQuantity(List<Troop> input);
        Task SaveAndRemoveTroopsLostAttackerAsync(List<AttackerTroops> attackerTroops, long battleId, Kingdom attacker);
        Task SaveAndRemoveTroopsLostDefenderAsync(List<Troop> input, long battleId);
    }
}
