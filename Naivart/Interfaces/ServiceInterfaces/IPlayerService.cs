using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    interface IPlayerService
    {
        Task<Player> RegisterPlayerAsync(string username, string password, string kingdomName);
        Task CreateBasicBuildingsAsync(long kingdomId);
        Task CreateResourcesAsync(long kingdomId);
    }
}
