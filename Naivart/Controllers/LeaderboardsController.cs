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
        public TroopService TroopService { get; set; }
        public KingdomService KingdomService { get; set; }
        public LeaderboardsController(BuildingService buildingService, TroopService troopService, KingdomService                                      kingdomService)
        {
            BuildingService = buildingService;
            TroopService = troopService;
            KingdomService = kingdomService;
        }

        [HttpGet("buildings")]
        public IActionResult BuildingsLeaderboard()
        {
            LeaderboardBuildingsAPIResponse response = new()
            {
                Results = BuildingService.GetBuildingsLeaderboard(out int status, out string error)
            };
            if (status != 200)
            {
                return StatusCode(status, new ErrorResponse() { error = error });
            }
            return Ok(response);
        }

        [HttpGet("troops")]
        public IActionResult TroopsLeaderboard()
        {
            LeaderboardTroopsAPIResponse response = new()
            {
                Results = TroopService.GetTroopsLeaderboard(out int status, out string error)
            };
            if (status != 200)
            {
                return StatusCode(status, new ErrorResponse() { error = error });
            }
            return Ok(response);
        }

        [HttpGet("kingdoms")]
        public IActionResult KingdomsLeaderboard()
        {
            LeaderboardKingdomsAPIResponse response = new()
            {
                results = KingdomService.GetKingdomsLeaderboard(out int status, out string error)
            };
            if (status != 200)
            {
                return StatusCode(status, new ErrorResponse() { error = error });
            }
            return Ok(response);
        }
    }
}
