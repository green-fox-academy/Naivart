using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IKingdomService KingdomService { get; set; }
        public ILoginService LoginService { get; set; }
        public IPlayerService PlayerService { get; set; }

        public HomeController(IKingdomService kingdomService, ILoginService loginService,
                              IPlayerService playerService)
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
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message))
                                        : Ok(new TokenWithStatusResponse("OK", result.message));
        }

        [HttpPost("registration")]
        public async Task<IActionResult> PlayerRegistrationAsync([FromBody] RegisterRequest request)
        {
            var model = await PlayerService.RegisterPlayerAsync(request);
            return model == null
                ? BadRequest(new ErrorResponse("Username was empty, already exists or password was shorter than 8 characters!"))
                : Ok(new RegisterResponse(model.Username, model.Kingdom.Id));
        }

        [Authorize]
        [HttpPut("registration")]
        public async Task<IActionResult> KingdomRegistrationAsync([FromBody] KingdomLocationInput input)
        {
            var result = await KingdomService.RegisterKingdomAsync(input, HttpContext.User.Identity.Name);
            return result.status != 200 ? StatusCode(result.status, new ErrorResponse(result.message)) 
                                        : Ok(new StatusResponse(result.message));
        }
    }
}
