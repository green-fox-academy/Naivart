using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Services;

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
        public IActionResult BuildingsLeaderboard()
        {
            LeaderboardBuildingsAPIResponse response = new()
            {
                Results = BuildingService.GetBuildingsLeaderboard(out int status, out string error)
            };
            return status != 200 ? StatusCode(status, new ErrorResponse() { Error = error })
                                 : Ok(response);
        }

        [HttpGet("troops")]
        public IActionResult TroopsLeaderboard()
        {
            LeaderboardTroopsAPIResponse response = new()
            {
                Results = TroopService.GetTroopsLeaderboard(out int status, out string error)
            };
            return status != 200 ? StatusCode(status, new ErrorResponse() { Error = error })
                                 : Ok(response);
        }

        [HttpGet("kingdoms")]
        public IActionResult KingdomsLeaderboard()
        {
            LeaderboardKingdomsAPIResponse response = new()
            {
                Results = KingdomService.GetKingdomsLeaderboard(out int status, out string error)
            };
            return status != 200 ? StatusCode(status, new ErrorResponse() { Error = error })
                                  : Ok(response);
        }
    }
}
