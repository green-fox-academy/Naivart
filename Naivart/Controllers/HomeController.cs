using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public HomeController(KingdomService kingdomService, PlayerService playerService)
        {
            PlayerService = playerService;
            KingdomService = kingdomService;
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
    }
}
