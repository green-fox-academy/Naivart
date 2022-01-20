using Microsoft.AspNetCore.Mvc;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Services;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("leaderboards")]
    public class LeaderboardsController : Controller
    {
        public IBuildingService BuildingService { get; set; }
        public IKingdomService KingdomService { get; set; }
        public ITroopService TroopService { get; set; }
        public LeaderboardsController(IBuildingService buildingService,
                                      IKingdomService kingdomService,
                                      ITroopService troopService)
        {
            BuildingService = buildingService;
            KingdomService = kingdomService;
            TroopService = troopService;
        }

        [HttpGet("buildings")]
        public async Task<IActionResult> BuildingsLeaderboardAsync()
        {
            var result = await BuildingService.GetBuildingsLeaderboardAsync();
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message))
                                        : Ok(new LeaderboardBuildingsAPIResponse(result.model));
        }

        [HttpGet("troops")]
        public async Task<IActionResult> TroopsLeaderboardAsync()
        {
            var result = await TroopService.GetTroopsLeaderboardAsync();
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message))
                                        : Ok(new LeaderboardTroopsAPIResponse(result.model));
        }

        [HttpGet("kingdoms")]
        public async Task<IActionResult> KingdomsLeaderboardAsync()
        {
            var result = await KingdomService.GetKingdomsLeaderboardAsync();
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message))
                                        : Ok(new LeaderboardKingdomsAPIResponse(result.model));
        }
    }
}
