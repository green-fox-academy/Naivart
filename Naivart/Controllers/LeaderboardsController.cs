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
        public KingdomService KingdomService { get; set; }
        public BuildingService BuildingService { get; set; }
        public LeaderboardsController(KingdomService kingdomService, BuildingService buildingService)
        {
            KingdomService = kingdomService;
            BuildingService = buildingService;
        }

        [HttpGet("buildings")]
        public IActionResult BuildingLeaderboard()
        {
            LeaderboardBuildingsAPIResponse response = new()
            {
                Results = BuildingService.GetBuildingLeaderboards(out int status, out string error)
            };
            if (status != 200)
            {
                return StatusCode(status, new ErrorResponse() { error = error });
            }
            return Ok(response);
        }
    }
}
