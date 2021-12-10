using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naivart.Interfaces;
using Naivart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlayerController : ControllerBase
    {
        private IAuthService authService;

        public PlayerController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = authService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var users = authService.GetAll();
            return Ok(users);
        }
    }
}
