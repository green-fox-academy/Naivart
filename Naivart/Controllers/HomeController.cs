using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;

        private KingdomService KingdomService { get; set; }

        private PlayerService PlayerService { get; set; }

        public HomeController(IMapper mapper, KingdomService kingdomService, PlayerService playerService)
        {
            _mapper = mapper;
            KingdomService = kingdomService;
            PlayerService = playerService;
        }

        [HttpGet("kingdoms")]
        public object Kingdoms()
        {
            var kingdoms = KingdomService.GetAll();
            var kingdomAPIModels = new List<KingdomAPIModel>();

            if (kingdoms.Count == 0)
            {
                return NotFound(new { errorMessage = "No data was present!" });
            }

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
                kingdomAPIModels.Add(kingdomAPIModel);
            }

            return Ok(new { kingdoms = kingdomAPIModels });
        }
    }
}
