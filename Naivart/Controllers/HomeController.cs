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
            string token = LoginService.Authenticate(player);
            if (token == "")
            {
                var emptyInput = new StatusForError(){ error = "Field username and/or field password was empty!" };
                return StatusCode(400, emptyInput);
            }
            else if (token is null)
            {
                var wrongLogin = new StatusForError() { error = "Username and/or password was incorrect!" };
                return StatusCode(401, wrongLogin);
            }
            var correctLogin = new TokenWithStatus() { status = "ok", token = token};
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
    }
}
