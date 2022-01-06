using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Services;
using System;
using System.Linq;

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
        public object KingdomsInformation()
        {
            var kingdoms = KingdomService.GetAll();
            var kingdomAPIModels = KingdomService.ListOfKingdomsMapping(kingdoms);
            var response = new KingdomsResponse() { Kingdoms = kingdomAPIModels };

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }

        [Authorize]
        [HttpGet("kingdoms/{id}")]
        public IActionResult KingdomInformation([FromRoute] long id)
        {
            var info = KingdomService.GetKingdomInfo(id, HttpContext.User.Identity.Name,
                out int status, out string error);

            return status != 200 ? StatusCode(status, new ErrorResponse() { Error = error })
                                 : Ok(info);
        }

        [Authorize]
        [HttpPut("kingdoms/{id}")]
        public object RenameKingdom([FromRoute] long id, [FromBody] RenameKingdomRequest request)
        {
            if (String.IsNullOrWhiteSpace(request.KingdomName))
            {
                return BadRequest(new ErrorResponse()
                { Error = "Field kingdomName was empty!" });
            }

            var kingdomWithTheSameName = KingdomService.GetAll().Where
                (k => k.Name == request.KingdomName).FirstOrDefault();

            if (kingdomWithTheSameName != null)
            {
                return BadRequest(new ErrorResponse()
                { Error = "Given kingdom name already exists!" });
            }

            if (!KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = KingdomService.RenameKingdom(id, request.KingdomName);
            return Ok(new RenameKingdomResponse()
            {
                KingdomId = kingdom.Id,
                KingdomName = kingdom.Name
            });
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/buildings")]
        public IActionResult Buildings([FromRoute] long id)
        {
            if (!KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = KingdomService.GetById(id);
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
        public IActionResult AddBuilding([FromRoute] long id, [FromBody] AddBuildingRequest input)
        {
            var response = BuildingService.AddBuilding(input, id, HttpContext.User.Identity.Name,
                out int status);
            return status != 200 ? StatusCode(status) : Ok(response);
        }

        [Authorize]
        [HttpPut("kingdoms/{kingdomId}/buildings/{buildingId}")]
        public IActionResult UpgradeBuilding([FromRoute] long kingdomId, [FromRoute] long buildingId)
        {
            if (!KingdomService.IsUserKingdomOwner(kingdomId, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player!" });
            }

            if (!KingdomService.GetById(kingdomId).Buildings.Any(b => b.Id == buildingId))
            {
                return BadRequest(new ErrorResponse()
                { Error = "There is no such building in this kingdom!" });
            }

            var upgradedBuilding = BuildingService.UpgradeBuilding (kingdomId, buildingId,
                out int statusCode, out string error);
            return statusCode != 200 ? StatusCode(statusCode, new ErrorResponse() { Error = error })
                                     : Ok(upgradedBuilding);
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/resources")]
        public object Resources([FromRoute] long id)
        {
            if (!KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = KingdomService.GetById(id);
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
        public IActionResult Troops([FromRoute] long id)
        {
            if (!KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse()
                { Error = "This kingdom does not belong to authenticated player" });
            }

            var kingdom = KingdomService.GetById(id);
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
        public IActionResult CreateTroops([FromRoute] long id, [FromBody] CreateTroopRequest input)
        {
            var model = TroopService.TroopCreateRequest(input, id, HttpContext.User.Identity.Name,
                out int status, out string result);
            return status != 200 ? StatusCode(status, new ErrorResponse() { Error = result })
                                     : Ok(model);
        }

        [Authorize]
        [HttpPut("kingdoms/{id}/troops")]
        public IActionResult UpgradeTroops([FromRoute] long id, [FromBody] UpgradeTroopsRequest input)
        {
            var troop = TroopService.UpgradeTroops(id, HttpContext.User.Identity.Name, input.Type, out string result, out int statusCode);
            if (statusCode != 200)
            {
                var errorResponse = new ErrorResponse() { Error = result };
                return StatusCode(statusCode, errorResponse);
            }
            var upgradeTroopsResponse = new UpgradeTroopsResponse() { status = "ok" };
            return Ok(upgradeTroopsResponse);
        }
    }
}

