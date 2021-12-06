using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public LoginService LoginService { get; set; }
        public HomeController(LoginService service)
        {
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
                return StatusCode(400);
            }
            else if (token is null)
            {
                return StatusCode(401);
            }
            return Ok();

        }
    }
}
