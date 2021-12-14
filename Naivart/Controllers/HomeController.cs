using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using Naivart.Services;

namespace Naivart.Controllers
{
    
    [Route("/")]
    public class HomeController : Controller
    {
        public ResourceService ResourceService { get; set; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public LoginService LoginService { get; set; }
        public BuildingService BuildingService { get; set; }
       
        public AuthService AuthService { get; set; }
        public TroopService TroopService { get; set; }
        public HomeController(ResourceService resourceService, KingdomService kingdomService, PlayerService playerService, LoginService loginService,AuthService authService, TroopService troopService, BuildingService buildingService)
        {
            ResourceService = resourceService;
            KingdomService = kingdomService;
            LoginService = loginService;
            PlayerService = playerService;
            AuthService = authService;
            TroopService = troopService;
            BuildingService = buildingService;
        }
        
        [HttpPost("registration")]
        public IActionResult Registration([FromBody] RegisterRequest request)
        {
            Player player = PlayerService.RegisterPlayer(
                request.username,
                request.password,
                request.kingdomName);
            if (player != null)
            {
                var response = new RegisterResponse()
                {
                    username = player.Username,
                    kingdomId = player.Kingdom.Id
                };
                return StatusCode(200, response);
            }
            else
            {
                var response = new ErrorResponse() { error = "Username was empty, already exists or password was shorter than 8 characters!" };
                return StatusCode(400, response);
            }
        }
        

        [HttpGet("kingdoms")]
        public object Kingdoms()
        {
            var kingdoms = KingdomService.GetAll();
            var kingdomAPIModels = KingdomService.ListOfKingdomsMapping(kingdoms);
            var response = new KingdomAPIResponse() { Kingdoms = kingdomAPIModels };

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }


        [Authorize]
        [HttpGet("kingdoms/{id}/troops")]
        public IActionResult Troops([FromRoute] long id)
        {
            if (!KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                ErrorResponse ErrorResponse = new ErrorResponse()
                { error = "This kingdom does not belong to authenticated player" };
                return Unauthorized(ErrorResponse);
            }

                var kingdom = KingdomService.GetByIdWithTroops(id);
                var kingdomApiModel = KingdomService.KingdomMapping(kingdom);
                var troopAPIModels = TroopService.ListOfTroopsMapping(kingdom.Troops);
                var response = new TroopAPIResponse()
                {
                    Kingdom = kingdomApiModel,
                    Troops = troopAPIModels
                };
                return Ok(response);
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/resources")]
        public object Resources([FromRoute] long id)
        {
            if (KingdomService.IsUserKingdomOwner(id, HttpContext.User.Identity.Name))
            {
                var kingdom = KingdomService.GetByIdWithResources(id);
                var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
                var resourceAPIModels = ResourceService.ListOfResourcesMapping(kingdom.Resources);
                var response = new ResourceAPIResponse()
                {
                    Kingdom = kingdomAPIModel,
                    Resources = resourceAPIModels
                };

                return Ok(response);
            }
            else
            {
                return Unauthorized(new { error = "This kingdom does not belong to authenticated player" });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] PlayerLogin player)
        {
            string tokenOrMessage = LoginService.Authenticate(player, out int statusCode);
            if (statusCode != 200)
            {
                var output = new StatusForError() { error = tokenOrMessage };
                return StatusCode(statusCode, output);
            }
            var correctLogin = new TokenWithStatus() { status = "ok", token = tokenOrMessage };
            return Ok(correctLogin);
        }
        
        [HttpPost("auth")]
        public IActionResult Auth([FromBody] PlayerIdentity token)
        {
            var player = LoginService.GetTokenOwner(token);
            if (player == null)
            {
                return StatusCode(401);
            }
            else
            {
                return Ok(player);
            }
        }

        [Authorize]
        [HttpGet("kingdoms/{id}/buildings")]
        public IActionResult Buildings([FromRoute] long id)
        {
            string result = HttpContext.User.Identity.Name;
            var response = BuildingService.GetBuildingResponse(id, HttpContext.User.Identity.Name,out int status);
            if (status != 200)
            {
                return StatusCode(401);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPut("registration")]
        public IActionResult KingdomRegistration([FromBody]KingdomLocationInput input)
        {
            string result = KingdomService.RegisterKingdom(input, HttpContext.User.Identity.Name, out int status);
            if (status != 200)
            {
                var outputError = new StatusForError() { error = result};
                return StatusCode(status, outputError);
            }
            var outputOk = new StatusOutput() { status = result };
            return Ok(outputOk);
        }
        
        [Authorize]
        [HttpGet("kingdoms/{id}")]
        public IActionResult KingdomInformation([FromRoute]long id)
        {
            var model = KingdomService.GetKingdomInfo(id, HttpContext.User.Identity.Name, out int status, out string error);
            if (status != 200)
            {
                return StatusCode(status, new StatusForError() { error = error });
            }
            return Ok(model);
        }
    }
}
