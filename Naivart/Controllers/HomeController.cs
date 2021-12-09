using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public LoginService LoginService { get; set; }
        public HomeController(IMapper mapper, KingdomService kingdomService, PlayerService playerService, LoginService service)
        {
            _mapper = mapper;
            KingdomService = kingdomService;
            LoginService = service;
            PlayerService = playerService;
        }

        [HttpGet("kingdoms")]
        public object Kingdoms()
        {
            var kingdoms = KingdomService.GetAll();
            var response = new KingdomAPIResponse(_mapper, kingdoms);

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] PlayerLogin player)
        {
            string tokenOrMessage = LoginService.Authenticate(player, out int statusCode);
            if (statusCode != 200)
            {
                var output = new StatusForError(){ error = tokenOrMessage };
                return StatusCode(statusCode, output);
            }
            var correctLogin = new TokenWithStatus() { status = "ok", token = tokenOrMessage};
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

        [HttpPut("registration")]
        public IActionResult KingdomRegistration([FromBody]KingdomLocationInput input)
        {
            string result = KingdomService.RegisterKingdom(input, out int status);
            if (status != 400)
            {
                var outputError = new StatusForError() { error = result};
                return StatusCode(status, outputError);
            }
            var outputOk = new StatusOutput() { status = result };
            return Ok(outputOk);
        }
    }
}
