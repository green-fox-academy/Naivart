using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Task<List<Kingdom>> GetAllKingdomsAsync();
        Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName);
        Task<Kingdom> FindPlayerInfoByKingdomIdAsync(long kingdomId);
        Task<Kingdom> KingdomIncludeResourceByIdAsync(long kingdomId);
        Task<Kingdom> FindKingdomByNameAsync(string kingdomName);
        Task<bool> HasAlreadyLocationAsync(KingdomLocationInput input);
        Task ChangeLocationIdForKingdomAsync(KingdomLocationInput input, long locationId);
        Task<bool> DoesKingdomExistAsync(long kingdomId);
        Task PopulationUp(long kingdomId, int quantity);
    }
}
