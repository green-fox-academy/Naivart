using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
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
        private ApplicationDbContext DbContext { get; }
        public BuildingService BuildingService { get; set; }
        public TimeService TimeService { get; set; }
        public PlayerService(IMapper mapper, ApplicationDbContext dbContext, BuildingService
                             buildingService, TimeService timeService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            BuildingService = buildingService;
            TimeService = timeService;
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

            var newKingdom = DbContext.Kingdoms.Add(kingdom).Entity;
            DbContext.SaveChanges();

            var DbKingdom = FindKingdomByName(kingdom.Name);
            player.KingdomId = DbKingdom.Id;
            var newPlayer = DbContext.Players.Add(player).Entity;
            DbContext.SaveChanges();
            CreateBasicBuildings(DbKingdom.Id); //creates basic buildings and save to Db 
            CreateResources(DbKingdom.Id);  //add resources to player (1000 gold and 0 food)

            return DbContext.Players.Include(x => x.Kingdom).FirstOrDefault
                (x => x.Username == username && x.Password == hashedPassword);
        }

        public Kingdom FindKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.FirstOrDefault(x => x.Name == kingdomName);
        }

        public Player FindByUsername(string username)
        {
            return DbContext.Players.FirstOrDefault(x => x.Username == username);
        }

        public bool IsInDbWithThisUsername(string username)
        {
            return DbContext.Players.Any(x => x.Username == username);
        }

        public void DeleteByUsername(string username)
        {
            DbContext.Players.Remove(FindByUsername(username));
        }

        public Player GetPlayerById(long id)
        {
            try
            {
                return DbContext.Players.Include(p => p.Kingdom)
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
            DbContext.Resources.Add(new Resource()
            {
                Type = "food",
                Amount = 0,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            DbContext.SaveChanges();

            DbContext.Resources.Add(new Resource()
            {
                Type = "gold",
                Amount = 1000,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            DbContext.SaveChanges();
        }
    }
}
