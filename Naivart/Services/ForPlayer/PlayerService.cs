using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Player RegisterPlayer(string username, string password, string kingdomName)
        {
            if (password.Length < 8
                || String.IsNullOrWhiteSpace(username)
                || IsInDbWithThisUsername(username))
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
            kingdom.Name = !String.IsNullOrWhiteSpace(kingdomName) && FindKingdomByName(kingdomName) == null
                           ? kingdomName : $"{player.Username}'s kingdom";

            UnitOfWork.Kingdoms.Add(kingdom);
            UnitOfWork.CompleteAsync();

            var DbKingdom = FindKingdomByName(kingdom.Name);
            player.KingdomId = DbKingdom.Id;
            UnitOfWork.Players.Add(player);
            UnitOfWork.CompleteAsync();
            CreateBasicBuildings(DbKingdom.Id); //creates basic buildings and save to Db 
            CreateResources(DbKingdom.Id);  //add resources to player (1000 gold and 0 food)

            return UnitOfWork.Players.Include(x => x.Kingdom).FirstOrDefault
                (x => x.Username == username && x.Password == hashedPassword);
        }

        public Kingdom FindKingdomByName(string kingdomName)
        {
            return UnitOfWork.Kingdoms.FirstOrDefault(x => x.Name == kingdomName);
        }

        public Player FindByUsername(string username)
        {
            return UnitOfWork.Players.FirstOrDefault(x => x.Username == username);
        }

        public bool IsInDbWithThisUsername(string username)
        {
            return UnitOfWork.Players.Any(x => x.Username == username);
        }

        public void DeleteByUsername(string username)
        {
            UnitOfWork.Players.Remove(FindByUsername(username));
        }

        public Player GetPlayerById(long id)
        {
            try
            {
                return UnitOfWork.Players.Include(p => p.Kingdom)
                                .FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public void CreateBasicBuildings(long kingdomId)
        {
            var townhallRequest = new BuildingRequest() { Type = "townhall" };
            var farmRequest = new BuildingRequest() { Type = "farm" };
            var mineRequest = new BuildingRequest() { Type = "mine" };
            var basicBuildings = new List<BuildingRequest> 
                { townhallRequest, farmRequest, mineRequest };

            foreach (var building in basicBuildings)
            {
                BuildingService.AddBasicBuilding(building, kingdomId);
            }
        }

        public void CreateResources(long kingdomId)
        {
            UnitOfWork.Resources.Add(new Resource()
            {
                Type = "food",
                Amount = 0,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            UnitOfWork.CompleteAsync();

            UnitOfWork.Resources.Add(new Resource()
            {
                Type = "gold",
                Amount = 1000,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            UnitOfWork.CompleteAsync();
        }
    }
}
