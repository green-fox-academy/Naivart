using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;
using System.Threading.Tasks;

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
        public async Task<IActionResult> AuthAsync([FromBody] PlayerIdentity token)
        {
            var player = await LoginService.GetTokenOwnerAsync(token);
            return player == null ? Unauthorized() : Ok(player);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] PlayerLogin player)
        {
            var result = await LoginService.AuthenticateAsync(player);
            if (result.status != 200)
            {   
                var output = new ErrorResponse() { Error = result.message };
                return StatusCode(result.status, output);
            }
            var correctLogin = new TokenWithStatusResponse()
            { Status = "OK", Token = result.message };
            return Ok(correctLogin);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> PlayerRegistrationAsync([FromBody] RegisterRequest request)
        {
            Player player = await PlayerService.RegisterPlayerAsync(
                   request.Username,
                   request.Password,
                   request.KingdomName);
            if (player == null)
            {
                var response = new ErrorResponse()
                {
                    Error = "Username was empty, already exists or password was shorter than 8 characters!"
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
        public async Task<IActionResult> KingdomRegistrationAsync([FromBody] KingdomLocationInput input)
        {
            var result = await KingdomService.RegisterKingdomAsync(input, HttpContext.User.Identity.Name);
            if (result.status != 200)
            {
                var outputError = new ErrorResponse() { Error = result.message };
                return StatusCode(result.status, outputError);
            }
            var outputOk = new StatusResponse() { Status = result.message };
            return Ok(outputOk);
        }
    }
}
