using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public ResourceService ResourceService { get; set; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public LoginService LoginService { get; set; }
        public HomeController(IMapper mapper, ResourceService resourceService, KingdomService kingdomService, PlayerService playerService, LoginService loginService)
        {
            _mapper = mapper;
            ResourceService = resourceService;
            KingdomService = kingdomService;
            PlayerService = playerService;
            LoginService = loginService;
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

        [HttpGet("kingdoms/{id}/resources")]
        public object Resources([FromRoute] long id)
        {
            var kingdom = KingdomService.GetById(id);
            var kingdomAPIModel = KingdomService.KingdomMapping(kingdom);
            var resourceAPIModels = ResourceService.ListOfResourcesMapping(kingdom.Resources);
            var response = new ResourceAPIResponse()
            {
                Kingdom = kingdomAPIModel,
                Resources = resourceAPIModels
            };

            return Ok(response);
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

        //[HttpGet("kingdoms/{id}/troops")]
        //public IActionResult Troops([FromRoute] long id)
        //{

        //}

    }
}
