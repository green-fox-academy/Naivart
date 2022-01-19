using Naivart.Models.APIModels;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface ILoginService
    {
        Task<(int status, string message)> AuthenticateAsync(PlayerLogin player);
        Task<bool> IsLoginPasswordCorrectAsync(string name, string password);
        Task<PlayerWithKingdom> GetTokenOwnerAsync(PlayerIdentity player);
        Task<PlayerWithKingdom> FindPlayerByNameAsync(string name);
        Task<PlayerInfo> GetTokenOwnerInfoAsync(string token);
    }
}
