using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {   
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public LoginService LoginService { get; set; }
        public HomeController(KingdomService kingdomService, PlayerService playerService,LoginService service)
        {
            LoginService = service;
            PlayerService = playerService;
            KingdomService = kingdomService;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpPut("registration")]
        public IActionResult KingdomRegistration([FromBody]KingdomLocationInput input)
        {
            string result = KingdomService.RegisterKingdom(input, out int status);
            if (status == 400)
            {
                var outputError = new StatusForError() { error = result};
                return StatusCode(status, outputError);
            }

            var outputOk = new StatusOutput() { status = result };
            return Ok(outputOk);
        }
    }
}
