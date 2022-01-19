using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Naivart.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IMapper _mapper;
        public IBuildingService BuildingService { get; set; }
        public ITimeService TimeService { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }
        public PlayerService(IMapper mapper, IBuildingService buildingService, ITimeService timeService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            BuildingService = buildingService;
            TimeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Player> RegisterPlayerAsync(RegisterRequest request)
        {
            if (request.Password.Length < 8 || String.IsNullOrWhiteSpace(request.Username)
                || await _unitOfWork.Players.IsInDbWithThisUsernameAsync(request.Username))
            {
                return null;
            }

            //install Microsoft.AspNet.WebPages nuget
            string salt = Crypto.GenerateSalt();
            request.Password += salt;
            string hashedPassword = Crypto.HashPassword(request.Password);

            var player = new Player(request.Username, hashedPassword, salt);
            var kingdom = new Kingdom(!String.IsNullOrWhiteSpace(request.KingdomName) &&
                                 await _unitOfWork.Kingdoms.FindKingdomByNameAsync(request.KingdomName)
                               == null ? request.KingdomName : $"{player.Username}'s kingdom");
            //check if given kingdom name is valid or already exists in database 

            _unitOfWork.Kingdoms.AddAsync(kingdom);
            await _unitOfWork.CompleteAsync();

            var newKingdom = await _unitOfWork.Kingdoms.FindKingdomByNameAsync(kingdom.Name);
            player.KingdomId = newKingdom.Id;
            _unitOfWork.Players.AddAsync(player);
            await _unitOfWork.CompleteAsync();

            await CreateBasicBuildingsAsync(newKingdom.Id); //create basic buildings and save to database 
            await CreateResourcesAsync(newKingdom.Id); //add resources to player (1000 gold and 0 food)

            return await _unitOfWork.Players.PlayerIncludeKingdomFindByUsernameAndPasswordAsync(player.Username, player.Password);
        }


        public async Task CreateBasicBuildingsAsync(long kingdomId)
        {
            var basicBuildings = new List<BuildingRequest> { new BuildingRequest("townhall"),
                new BuildingRequest("farm"), new BuildingRequest("mine")};

            foreach (var building in basicBuildings)
            {
                await BuildingService.AddBasicBuildingAsync(building, kingdomId);
            }
        }

        public async Task CreateResourcesAsync(long kingdomId)
        {
            _unitOfWork.Resources.AddAsync(new Resource("food", 0, 1, TimeService.GetUnixTimeNow(), kingdomId));
            await _unitOfWork.CompleteAsync();

            _unitOfWork.Resources.AddAsync(new Resource("gold", 1000, 1, TimeService.GetUnixTimeNow(), kingdomId));
            await _unitOfWork.CompleteAsync();
        }
    }
}
