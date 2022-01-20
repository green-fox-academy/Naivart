using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface IPlayerService
    {
        Task<Player> RegisterPlayerAsync(RegisterRequest request);
        Task CreateBasicBuildingsAsync(long kingdomId);
        Task CreateResourcesAsync(long kingdomId);
    }
}
