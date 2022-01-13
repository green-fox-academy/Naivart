using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Naivart.Services
{
    public class PlayerService
    {
        private readonly IMapper mapper;
        public BuildingService BuildingService { get; set; }
        public TimeService TimeService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public PlayerService(IMapper mapper, BuildingService
                             buildingService, IUnitOfWork unitOfWork , TimeService timeService)
        {
            this.mapper = mapper;
            BuildingService = buildingService;
            TimeService = timeService;
            UnitOfWork = unitOfWork;
        }

        public async Task<Player> RegisterPlayerAsync(string username, string password, string kingdomName)
        {
            if (password.Length < 8
                || String.IsNullOrWhiteSpace(username)
                || await UnitOfWork.Players.IsInDbWithThisUsernameAsync(username))
            {
                return null;
            }

            //install Microsoft.AspNet.WebPages nuget
            string salt = Crypto.GenerateSalt();
            password = password + salt;
            string hashedPassword = Crypto.HashPassword(password);

            Player player = new Player() { Username = username, Password = hashedPassword, Salt = salt };
            Kingdom kingdom = new Kingdom();

            //check if given kingdom name (and username) is not empty or already exists in database 
            kingdom.Name = !String.IsNullOrWhiteSpace(kingdomName) && await UnitOfWork.Kingdoms.FindKingdomByNameAsync(kingdomName) == null
                           ? kingdomName : $"{player.Username}'s kingdom";

            UnitOfWork.Kingdoms.AddAsync(kingdom);
            await UnitOfWork.CompleteAsync();

            var DbKingdom = await UnitOfWork.Kingdoms.FindKingdomByNameAsync(kingdom.Name);
            player.KingdomId = DbKingdom.Id;
            UnitOfWork.Players.AddAsync(player);
            await UnitOfWork.CompleteAsync();
            await CreateBasicBuildingsAsync(DbKingdom.Id); //creates basic buildings and save to Db 
            await CreateResourcesAsync(DbKingdom.Id);  //add resources to player (1000 gold and 0 food)

            return await UnitOfWork.Players.PlayerInsludeKingdomFindByUsernameAndPasswordAsync(username, hashedPassword);
        }


        public async Task CreateBasicBuildingsAsync(long kingdomId)
        {
            var townhallRequest = new BuildingRequest() { Type = "townhall" };
            var farmRequest = new BuildingRequest() { Type = "farm" };
            var mineRequest = new BuildingRequest() { Type = "mine" };
            var basicBuildings = new List<BuildingRequest> 
                { townhallRequest, farmRequest, mineRequest };

            foreach (var building in basicBuildings)
            {
                await BuildingService.AddBasicBuildingAsync(building, kingdomId);
            }
        }

        public async Task CreateResourcesAsync(long kingdomId)
        {
            UnitOfWork.Resources.AddAsync(new Resource()
            {
                Type = "food",
                Amount = 0,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            await UnitOfWork.CompleteAsync();

            UnitOfWork.Resources.AddAsync(new Resource()
            {
                Type = "gold",
                Amount = 1000,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            await UnitOfWork.CompleteAsync();
        }
    }
}
