using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Kingdom;
using Naivart.Models.APIModels.Troops;
using Naivart.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("kingdoms")]
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

        [HttpGet("")]
        public async Task<object> KingdomsInformationAsync()
        {
            var kingdoms = await KingdomService.GetAllAsync();
            var kingdomAPIModels = KingdomService.ListOfKingdomsMapping(kingdoms);
            var response = new KingdomsResponse(kingdomAPIModels);

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> KingdomInformationAsync([FromRoute] long id)
        {
            var info = await KingdomService.GetKingdomInfoAsync(id, HttpContext.User.Identity.Name);

            return info.status != 200 ? StatusCode(info.status, new ErrorResponse(info.message))
                                      : Ok(info.model);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<object> RenameKingdomAsync([FromRoute] long id, [FromBody] RenameKingdomRequest request)
        {
            if (String.IsNullOrWhiteSpace(request.KingdomName))
            {
                return BadRequest(new ErrorResponse("Field kingdomName was empty!"));
            }

            var kingdoms = await KingdomService.GetAllAsync();
            var kingdomWithTheSameName = kingdoms.FirstOrDefault(k => k.Name == request.KingdomName);

            if (kingdomWithTheSameName != null)
            {
                return BadRequest(new ErrorResponse("Given kingdom name already exists!"));
            }

            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player"));
            }

            var kingdom = await KingdomService.RenameKingdomAsync(id, request.KingdomName);
            return Ok(new RenameKingdomResponse(kingdom.Id, kingdom.Name));
        }

        [Authorize]
        [HttpGet("{id}/buildings")]
        public async Task<IActionResult> BuildingsAsync([FromRoute] long id)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player"));
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var buildingAPIModels = BuildingService.ListOfBuildingsMapping(kingdom.Buildings);

            return Ok(new BuildingsResponse(kingdomAPIModel, buildingAPIModels));
        }

        [Authorize]
        [HttpPost("{id}/buildings")]
        public async Task<IActionResult> AddBuildingAsync([FromRoute] long id, [FromBody] BuildingRequest request)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player!"));
            }

            if (string.IsNullOrEmpty(request.Type))
            {
                return BadRequest(new ErrorResponse("Type is required."));
            }

            if (!await BuildingService.IsBuildingTypeDefinedAsync(request.Type))
            {
                return BadRequest(new ErrorResponse("Type is unknown."));
            }

            var response = await BuildingService.AddBuildingAsync(request, id);
            return response.status != 200 ? StatusCode(response.status, new ErrorResponse(response.message))
                                          : Ok(response.model);
        }

        [Authorize]
        [HttpPut("{kingdomId}/buildings/{buildingId}")]
        public async Task<IActionResult> UpgradeBuildingAsync([FromRoute] long kingdomId, [FromRoute] long buildingId)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(kingdomId, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player!"));
            }

            var kingdom = await KingdomService.GetByIdAsync(kingdomId);
            if (!kingdom.Buildings.Any(b => b.Id == buildingId))
            {
                return BadRequest(new ErrorResponse("There is no such building in this kingdom!"));
            }

            var response = await BuildingService.UpgradeBuildingAsync(kingdomId, buildingId);
            return response.status != 200 ? StatusCode(response.status, new ErrorResponse(response.message))
                                          : Ok(response.model);
        }

        [Authorize]
        [HttpGet("{id}/resources")]
        public async Task<object> ResourcesAsync([FromRoute] long id)
        {
            if (!await KingdomService.IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player"));
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var resourceAPIModels = ResourceService.ListOfResourcesMapping(kingdom.Resources);
            return Ok(new ResourcesResponse(kingdomAPIModel, resourceAPIModels));
        }

        [Authorize]
        [HttpGet("{id}/troops")]
        public async Task<IActionResult> TroopsAsync([FromRoute] long id)
        {
            if (!await KingdomService .IsUserKingdomOwnerAsync(id, HttpContext.User.Identity.Name))
            {
                return Unauthorized(new ErrorResponse("This kingdom does not belong to authenticated player"));
            }

            var kingdom = await KingdomService.GetByIdAsync(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var troopAPIModels = TroopService.ListOfTroopsMapping(kingdom.Troops);
            return Ok(new TroopsResponse(kingdomAPIModel, troopAPIModels));
        }

        [HttpPost("{id}/troops")]
        public async Task<IActionResult> CreateTroopsAsync([FromRoute] long id, [FromBody] CreateTroopRequest input)
        {
            var response = await TroopService.TroopCreateRequestAsync(input, id, HttpContext.User.Identity.Name);
            return response.status != 200 ? StatusCode(response.status, new ErrorResponse(response.message))
                                          : Ok(response.list);
        }

        [Authorize]
        [HttpPut("{id}/troops")]
        public async Task<IActionResult> UpgradeTroopsAsync([FromRoute] long id, [FromBody] UpgradeTroopsRequest input)
        {
            var result = await TroopService.UpgradeTroopsAsync(id, HttpContext.User.Identity.Name, input.Type);
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message)) 
                                        : Ok(new UpgradeTroopsResponse("OK"));
        }

        [Authorize]
        [HttpPost("{id}/battles")]
        public async Task<IActionResult> BattlesAsync([FromRoute] long id, [FromBody] BattleTargetRequest input)
        {
            var response = await KingdomService.BattleAsync(input, id, HttpContext.User.Identity.Name);
            return response.status != 200 ? StatusCode(response.status, new ErrorResponse(response.message))
                                          : Ok(response.model);
        }
        
        [Authorize]
        [HttpGet("{kingdomId}/battles/{id}")]
        public async Task<IActionResult> BattleResultAsync([FromRoute] long kingdomId, long id)
        {
            var response = await KingdomService.BattleInfoAsync(id, kingdomId, HttpContext.User.Identity.Name);
            return response.status != 200 ? StatusCode(response.status, new ErrorResponse(response.message))
                                          : Ok(response.model);
        }
    }
}

