using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface IPlayerService
    {
        Task<Player> RegisterPlayerAsync(string username, string password, string kingdomName);
        Task CreateBasicBuildingsAsync(long kingdomId);
        Task CreateResourcesAsync(long kingdomId);
    }
}
