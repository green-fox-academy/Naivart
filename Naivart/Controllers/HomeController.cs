using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public KingdomService KingdomService { get; set; }
        public LoginService LoginService { get; set; }
        public PlayerService PlayerService { get; set; }

        public HomeController(KingdomService kingdomService, LoginService loginService,
                              PlayerService playerService)
        {
            KingdomService = kingdomService;
            LoginService = loginService;
            PlayerService = playerService;
        }

        [HttpPost("auth")]
        public IActionResult Auth([FromBody] PlayerIdentity token)
        {
            var player = LoginService.GetTokenOwner(token);
            return player == null ? Unauthorized() : Ok(player);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] PlayerLogin player)
        {
            string tokenOrMessage = LoginService.Authenticate(player, out int statusCode);
            if (statusCode != 200)
            {   
                var output = new ErrorResponse() { Error = tokenOrMessage };
                return StatusCode(statusCode, output);
            }
            var correctLogin = new TokenWithStatusResponse()
            { Status = "ok", Token = tokenOrMessage };
            return Ok(correctLogin);
        }

        [HttpPost("registration")]
        public IActionResult PlayerRegistration([FromBody] RegisterRequest request)
        {
            Player player = PlayerService.RegisterPlayer(
                request.Username,
                request.Password,
                request.KingdomName);
            if (player == null)
            {
                var response = new ErrorResponse()
                {
                    Error =
                "Username was empty, already exists or password was shorter than 8 characters!"
                };
                return BadRequest(response);
            }
            else
            {
                var response = new RegisterResponse()
                {
                    Username = player.Username,
                    KingdomId = player.Kingdom.Id
                };
                return Ok(response);
            }
        }

        [Authorize]
        [HttpPut("registration")]
        public IActionResult KingdomRegistration([FromBody] KingdomLocationInput input)
        {
            string result = KingdomService.RegisterKingdom(input, HttpContext.User.Identity.Name,
                out int status);
            if (status != 200)
            {
                var outputError = new ErrorResponse() { Error = result };
                return StatusCode(status, outputError);
            }
            var outputOk = new StatusResponse() { Status = result };
            return Ok(outputOk);
        }
    }
}
