using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public LoginService LoginService { get; set; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public HomeController(KingdomService kingdomService, PlayerService playerService, LoginService loginService)
        {
            PlayerService = playerService;
            KingdomService = kingdomService;
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
    }
}
