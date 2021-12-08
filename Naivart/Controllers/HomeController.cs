﻿using Microsoft.AspNetCore.Mvc;
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
        public HomeController(KingdomService kingdomService, PlayerService playerService, LoginService loginservice)
        {
            PlayerService = playerService;
            KingdomService = kingdomService;
            LoginService = loginservice;
        }

        public IActionResult Index()
        {
            return View();
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
    }
}
