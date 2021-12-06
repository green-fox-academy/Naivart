using Microsoft.AspNetCore.Mvc;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    public class HomeController : Controller
    {
        public PlayerService PlayerService { get; set; }
        public HomeController(PlayerService playerService)
        {
            PlayerService = playerService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
