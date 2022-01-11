using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("/")]
    public class KingdomController : Controller
    {
        public BuildingService BuildingService { get; set; }
        public KingdomService KingdomService { get; set; }
        public ResourceService ResourceService { get; set; }
        public TroopService TroopService { get; set; }

        public KingdomController(BuildingService buildingService, KingdomService kingdomService,
                                 ResourceService resourceService, TroopService troopService)
        {
            BuildingService = buildingService;
            KingdomService = kingdomService;
            ResourceService = resourceService;
            TroopService = troopService;
        }

        [HttpGet("kingdoms")]
        public async Task<object> KingdomsInformationAsync()
        {
            var kingdoms = await KingdomService.GetAllAsync();
            var kingdomAPIModels = KingdomService.ListOfKingdomsMapping(kingdoms);
            var response = new KingdomsResponse() { Kingdoms = kingdomAPIModels };

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }

        [Authorize]
        [HttpGet("kingdoms/{id}")]
        public async Task<IActionResult> KingdomInformationAsync([FromRoute] long id)
        {
            var info = await KingdomService.GetKingdomInfoAsync(id, HttpContext.User.Identity.Name);

            return info.Item2 != 200 ? StatusCode(info.Item2, new ErrorResponse() { Error = info.Item3 })
                                     : Ok(info.Item1);
        }

        [Authorize]
        [HttpPut("kingdoms/{id}")]
        public async Task<object> RenameKingdomAsync([FromRoute] long id, [FromBody] RenameKingdomRequest request)
        {
            if (String.IsNullOrWhiteSpace(request.KingdomName))
            {
                return BadRequest(new ErrorResponse()
                { Error = "Field kingdomName was empty!" });
            }

            var kingdoms = await KingdomService.GetAllAsync();
            var kingdomWithTheSameName = kingdoms.Where(k => k.Name == request.KingdomName).FirstOrDefault();

            if (kingdomWithTheSameName != null)
            {
                return BadRequest(new ErrorResponse()
                { Error = "Given kingdom name already exists!" });
            }

            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = await KingdomService.RenameKingdomAsync(id, request.KingdomName);
            return Ok(new RenameKingdomResponse()
            {
                KingdomId = kingdom.Id,
                KingdomName = kingdom.Name
            });
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/buildings")]
        public async Task<IActionResult> BuildingsAsync([FromRoute] long id)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var buildingAPIModels = BuildingService.ListOfBuildingsMapping(kingdom.Buildings);
            var response = new BuildingsResponse()
            {
                Kingdom = kingdomAPIModel,
                Buildings = buildingAPIModels
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("kingdoms/{id}/buildings")]
        public async Task<IActionResult> AddBuildingAsync([FromRoute] long id, [FromBody] BuildingRequest request)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player!" });
            }

            if (string.IsNullOrEmpty(request.Type))
            {
                return BadRequest(new ErrorResponse() { Error = "Type is required." });
            }

            if (!await BuildingService.IsBuildingTypeDefinedAsync(request.Type))
            {
                return BadRequest(new ErrorResponse() { Error = "Type is unknown." });
            }

            var response = await BuildingService.AddBuildingAsync(request, id);
            return response.Item2 != 200 ? StatusCode(response.Item2, new ErrorResponse() { Error = response.Item3 })
                                         : Ok(response.Item1);
        }

        [Authorize]
        [HttpPut("kingdoms/{kingdomId}/buildings/{buildingId}")]
        public async Task<IActionResult> UpgradeBuildingAsync([FromRoute] long kingdomId, [FromRoute] long buildingId)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(kingdomId, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player!" });
            }

            var kingdom = await KingdomService.GetByIdAsync(kingdomId);
            if (!kingdom.Buildings.Any(b => b.Id == buildingId))
            {
                return BadRequest(new ErrorResponse()
                { Error = "There is no such building in this kingdom!" });
            }

            var response = await BuildingService.UpgradeBuildingAsync(kingdomId, buildingId);
            return response.Item2 != 200 ? StatusCode(response.Item2, new ErrorResponse() { Error = response.Item3 })
                                         : Ok(response.Item1);
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/resources")]
        public async Task<object> ResourcesAsync([FromRoute] long id)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var resourceAPIModels = ResourceService.ListOfResourcesMapping(kingdom.Resources);
            var response = new ResourcesResponse()
            {
                Kingdom = kingdomAPIModel,
                Resources = resourceAPIModels
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/troops")]
        public async Task<IActionResult> TroopsAsync([FromRoute] long id)
        {
            if (!await KingdomService .IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var troopAPIModels = TroopService.ListOfTroopsMapping(kingdom.Troops);
            var response = new TroopsResponse()
            {
                Kingdom = kingdomAPIModel,
                Troops = troopAPIModels
            };
            return Ok(response);
        }

        [HttpPost("kingdoms/{id}/troops")]
        public async Task<IActionResult> CreateTroopsAsync([FromRoute] long id, [FromBody] CreateTroopRequest input)
        {
            var response = await TroopService.TroopCreateRequestAsync(input, id, HttpContext.User.Identity.Name);
            return response.Item2 != 200 ? StatusCode(response.Item2, new ErrorResponse() { Error = response.Item3 })
                                         : Ok(response.Item1);
        }

        [Authorize]
        [HttpPut("kingdoms/{id}/troops")]
        public async Task<IActionResult> UpgradeTroopsAsync([FromRoute] long id, [FromBody] UpgradeTroopsRequest input)
        {
            var result = await TroopService.UpgradeTroopsAsync(id, HttpContext.User.Identity.Name, input.Type);
            if (result.Item1 != 200)
            {
                var errorResponse = new ErrorResponse() { Error = result.Item2 };
                return StatusCode(result.Item1, errorResponse);
            }
            var upgradeTroopsResponse = new UpgradeTroopsResponse() { status = "OK" };
            return Ok(upgradeTroopsResponse);
        }
    }
}

