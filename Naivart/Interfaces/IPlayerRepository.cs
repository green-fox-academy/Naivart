using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<bool> IsKingdomOwnerAsync(long kingdomId, string username);
        Task<Player> PlayerIncludeKingdomFindByUsernameAndPasswordAsync(string username, string hashedPassword);
        Task<Player> GetPlayerByIdAsync(long id);
        Task<bool> IsInDbWithThisUsernameAsync(string username);
        Task<Player> FindByUsernameAsync(string username);
        Task<Player> FindPlayerIncludeKingdomsByUsernameAsync(string name);
        Task<PlayerInfo> FindPlayerByNameReturnPlayerInfoAsync(string name);
        Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username);
    }
}
