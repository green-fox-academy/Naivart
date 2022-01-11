using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Services;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("leaderboards")]
    public class LeaderboardsController : Controller
    {
        public BuildingService BuildingService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TroopService TroopService { get; set; }
        public LeaderboardsController(BuildingService buildingService,
                                      KingdomService kingdomService,
                                      TroopService troopService)
        {
            BuildingService = buildingService;
            KingdomService = kingdomService;
            TroopService = troopService;
        }

        [HttpGet("buildings")]
        public async Task<IActionResult> BuildingsLeaderboardAsync()
        {
            var result = await BuildingService.GetBuildingsLeaderboardAsync();
            var response = new LeaderboardBuildingsAPIResponse()
            {
                Results = result.Item1
            };
            return result.Item2 != 200 ? StatusCode(result.Item2, new ErrorResponse() { Error = result.Item3 })
                                       : Ok(response);
        }

        [HttpGet("troops")]
        public async Task<IActionResult> TroopsLeaderboardAsync()
        {
            var result = await TroopService.GetTroopsLeaderboardAsync();
            var response = new LeaderboardTroopsAPIResponse()
            {
                Results = result.Item1
            };
            return result.Item2 != 200 ? StatusCode(result.Item2, new ErrorResponse() { Error = result.Item3 })
                                       : Ok(response);
        }

        [HttpGet("kingdoms")]
        public async Task<IActionResult> KingdomsLeaderboardAsync()
        {
            var result = await KingdomService.GetKingdomsLeaderboardAsync();
            var response = new LeaderboardKingdomsAPIResponse()
            {
                Results = result.Item1
            };
            return result.Item2 != 200 ? StatusCode(result.Item2, new ErrorResponse() { Error = result.Item3 })
                                       : Ok(response);
        }
    }
}
