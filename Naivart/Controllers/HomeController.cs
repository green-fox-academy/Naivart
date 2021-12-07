using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public LoginService LoginService { get; set; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public HomeController(KingdomService kingdomService, PlayerService playerService, LoginService service)
        {
            PlayerService = playerService;
            KingdomService = kingdomService;
            LoginService = service;
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
    }
}
