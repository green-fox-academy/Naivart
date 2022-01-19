using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface IBuildingService
    {
        List<BuildingAPIModel> ListOfBuildingsMapping(List<Building> buildings);
        Task<(BuildingResponse model, int status, string message)> AddBuildingAsync(BuildingRequest request, long kingdomId);
        Task<int> GetTownhallLevelAsync(long kingdomId);
        Task<(BuildingAPIModel model, int status, string message)> UpgradeBuildingAsync(long kingdomId, long buildingId);
        Task<(List<LeaderboardBuildingAPIModel> model, int status, string message)> GetBuildingsLeaderboardAsync();
        Task<bool> IsBuildingTypeDefinedAsync(string type);
        Task AddBasicBuildingAsync(BuildingRequest request, long kingdomId);
    }
}
