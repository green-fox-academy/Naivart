using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Kingdom;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface IKingdomService
    {
        Task<List<Kingdom>> GetAllAsync();
        Task<Kingdom> GetByIdAsync(long id);
        KingdomAPIModel KingdomMapping(Kingdom kingdom);
        List<KingdomAPIModel> ListOfKingdomsMapping(List<Kingdom> kingdoms);
        Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName);
        Task<(int status, string message)> RegisterKingdomAsync(KingdomLocationInput input, string usernameToken);
        bool IsCorrectLocationRange(KingdomLocationInput input);
        Task ConnectLocationAsync(KingdomLocationInput input);
        Task<(KingdomDetails model, int status, string message)> GetKingdomInfoAsync(long kingdomId, string tokenUsername);
        Task<KingdomDetails> GetAllInfoAboutKingdomAsync(long kingdomId);
        Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username);
        Task<int> GetGoldAmountAsync(long kingdomId);
        Task<(List<LeaderboardKingdomAPIModel> model, int status, string message)> GetKingdomsLeaderboardAsync();
        Task<(BattleTargetResponse model, int status, string message)> BattleAsync(BattleTargetRequest targetKingdom, long attackerId, string tokenUsername);
        Task<(BattleResultResponse model, int status, string message)> BattleInfoAsync(long battleId, long kingdomId, string tokenUsername);
        Task<BattleResultResponse> GetBattleInfoAsync(long battleId);
        Task<BattleTargetResponse> StartBattleAsync(BattleTargetRequest targetKingdom, Kingdom attacker);
        bool TroopQuantityCheck(BattleTargetRequest targetKingdom, Kingdom attacker);
        List<TroopBattleInfo> GetTroopQuantity(List<Troop> input);
        Task<long> CountTravelTimeAsync(long kingdomIdDef, long kingdomIdAtt);
        List<TroopBattleInfo> GetTroopLevels(List<Troop> input);
    }
}
