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
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public HomeController(KingdomService kingdomService, PlayerService playerService)
        {
            PlayerService = playerService;
            KingdomService = kingdomService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
