using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
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

        public async Task<Player> RegisterPlayerAsync(string username, string password, string kingdomName)
        {
            if (password.Length < 8
                || String.IsNullOrWhiteSpace(username)
                || await IsInDbWithThisUsernameAsync(username))
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
            kingdom.Name = !String.IsNullOrWhiteSpace(kingdomName) && FindKingdomByNameAsync(kingdomName) == null
                           ? kingdomName : $"{player.Username}'s kingdom";

            var newKingdom = await Task.FromResult(DbContext.Kingdoms.Add(kingdom).Entity);
            await DbContext.SaveChangesAsync();

            var DbKingdom = await FindKingdomByNameAsync(kingdom.Name);
            player.KingdomId = DbKingdom.Id;
            var newPlayer = await Task.FromResult(DbContext.Players.Add(player).Entity);
            await DbContext.SaveChangesAsync();
            await CreateBasicBuildingsAsync(DbKingdom.Id); //creates basic buildings and save to Db 
            await CreateResourcesAsync(DbKingdom.Id);  //add resources to player (1000 gold and 0 food)

            return await Task.FromResult(DbContext.Players.Include(x => x.Kingdom).FirstOrDefault
                (x => x.Username == username && x.Password == hashedPassword));
        }

        public async Task<Kingdom> FindKingdomByNameAsync(string kingdomName)
        {
            return await Task.FromResult(DbContext.Kingdoms.FirstOrDefault(x => x.Name == kingdomName));
        }

        public async Task<Player> FindByUsernameAsync(string username)
        {
            return await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.Username == username));
        }

        public async Task<bool> IsInDbWithThisUsernameAsync(string username)
        {
            return await Task.FromResult(DbContext.Players.Any(x => x.Username == username));
        }

        public async Task DeleteByUsernameAsync(string username)
        {
            await Task.FromResult(DbContext.Players.Remove(await FindByUsernameAsync(username)));
        }

        public async Task<Player> GetPlayerByIdAsync(long id)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.Include(p => p.Kingdom)
                                 .FirstOrDefault(p => p.Id == id));
            }
            catch
            {
                return null;
            }
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
            await DbContext.Resources.AddAsync(new Resource()
            {
                Type = "food",
                Amount = 0,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            await DbContext.SaveChangesAsync();

            await DbContext.Resources.AddAsync(new Resource()
            {
                Type = "gold",
                Amount = 1000,
                Generation = 1,
                UpdatedAt = TimeService.GetUnixTimeNow(),
                KingdomId = kingdomId
            });
            await DbContext.SaveChangesAsync();
        }
    }
}
