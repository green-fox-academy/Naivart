using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Services;

namespace Naivart.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)

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
            var response = new KingdomAPIResponse(_mapper, kingdoms);

            return response.Kingdoms.Count == 0 ? NotFound(new { kingdoms = response.Kingdoms })
                                                : Ok(new { kingdoms = response.Kingdoms });
        }
    }
}
