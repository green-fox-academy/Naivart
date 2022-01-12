using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<bool> IsLocationTakenAsync(KingdomLocationInput input);
        Task<long> GetLocationIdFromCoordinatesAsync(Location model);
        Task<Location> LocationAttAsync(long kingdomIdAtt);
        Task<Location> LocationDefAsync(long kingdomIdDef);
    }
}
