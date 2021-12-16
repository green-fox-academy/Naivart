using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("leaderboards")]
    public class LeaderboardsController : Controller
    {
        public BuildingService BuildingService { get; set; }
        public TroopService TroopService { get; set; }
        public LeaderboardsController(BuildingService buildingService, TroopService                                    troopService)
        {
            BuildingService = buildingService;
            TroopService = troopService;
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
    }
}
