using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class KingdomService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public LoginService LoginService { get; set; }
        //public BuildingService BuildingService { get; set; }
        public KingdomService(IMapper mapper, ApplicationDbContext dbContext,
                              AuthService authService, LoginService loginService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            AuthService = authService;
            LoginService = loginService;
        }

        public async Task<List<Kingdom>> GetAllAsync()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return await Task.FromResult(DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .Include(k => k.Buildings)
                    .Include(k => k.Resources)
                    .Include(k => k.Troops)
                    .ThenInclude(k => k.TroopType)
                    .ToList());
            }
            catch
            {
                return kingdoms;
            }
        }

        public async Task<Kingdom> GetByIdAsync(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                var kingdoms = await GetAllAsync();
                return await Task.FromResult(kingdoms.FirstOrDefault(k => k.Id == id));
            }
            catch
            {
                return kingdom;
            }
        }

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = mapper.Map<KingdomAPIModel>(kingdom);
            var locationAPIModel = mapper.Map<LocationAPIModel>(kingdom.Location);
            kingdomAPIModel.Location = locationAPIModel;
            return kingdomAPIModel;
        }
        public List<KingdomAPIModel> ListOfKingdomsMapping(List<Kingdom> kingdoms)
        {
            var kingdomAPIModels = new List<KingdomAPIModel>();
            if (kingdoms is null)
            {
                return kingdomAPIModels;
            }

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = KingdomMapping(kingdom);
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
        }

        public async Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName)
        {
            var kingdom = await GetByIdAsync(kingdomId);
            kingdom.Name = newKingdomName;
            DbContext.Update(kingdom);
            await DbContext.SaveChangesAsync();
            return kingdom;
        }

        public async Task<ValueTuple<int, string>> RegisterKingdomAsync(KingdomLocationInput input, string usernameToken)
        {
            try
            {
                //status = 400;
                if (!IsCorrectLocationRange(input))
                {
                    return (400, "One or both coordinates are out of valid range (0-99).");
                }
                else if (await IsLocationTakenAsync(input))
                {
                    return (400, "Given coordinates are already taken!");
                }
                else if (await HasAlreadyLocationAsync(input))
                {
                    if (await IsUserKingdomOwnerAsync(input.KingdomId, usernameToken))
                    {
                        await ConnectLocationAsync(input);
                        //status = 200;
                        return (200, "OK");
                    }
                    //status = 401;
                    return (401, "Wrong user authentication!");
                }
                //status = 409;
                return (409, "Your kingdom already have location!");
            }
            catch
            {
                //status = 500;
                return (500, "Data could not be read");
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.CoordinateX >= 0 && input.CoordinateX <= 99 && input.CoordinateY >= 0
                                          && input.CoordinateY <= 99;
        }

        public async Task<bool> IsLocationTakenAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(DbContext.Locations.Any(x => x.CoordinateX == input.CoordinateX 
                && x.CoordinateY == input.CoordinateY));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> HasAlreadyLocationAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(DbContext.Kingdoms.Any(x => x.Id == input.KingdomId 
                && x.LocationId == null));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task ConnectLocationAsync(KingdomLocationInput input)
        {
            try
            {
                var model = new Location() { CoordinateX = input.CoordinateX, CoordinateY = input.CoordinateY };
                await DbContext.Locations.AddAsync(model);
                await DbContext.SaveChangesAsync();
                long locationId = await Task.FromResult(DbContext.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX 
                && x.CoordinateY == model.CoordinateY).Id);
                await Task.FromResult(DbContext.Kingdoms.FirstOrDefault(x => x.Id == input.KingdomId).LocationId = locationId);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<ValueTuple<KingdomDetails, int, string>> GetKingdomInfoAsync(long kingdomId, string tokenUsername)
        {
            try
            {
                if (await IsUserKingdomOwnerAsync(kingdomId, tokenUsername))
                {
                    //error = "ok";
                    //status = 200;
                    return (await GetAllInfoAboutKingdomAsync(kingdomId), 200, "OK"); //this will use automapper to create an object
                }
                else
                {
                    //error = "This kingdom does not belong to authenticated player";
                    //status = 401;
                    return (null, 401, "This kingdom does not belong to authenticated player");
                }
            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<KingdomDetails> GetAllInfoAboutKingdomAsync(long kingdomId)
        {
            try
            {
                var kingdom = await GetByIdAsync(kingdomId);
                var buildings = new List<BuildingAPIModel>();
                var resources = new List<ResourceAPIModel>();
                var troops = new List<TroopInfo>();

                foreach (var building in kingdom.Buildings)   //list of buildings
                {
                    var buildingAPIModel = mapper.Map<BuildingAPIModel>(building);
                    buildings.Add(buildingAPIModel);
                }
                foreach (var resource in kingdom.Resources)   //list of resources
                {
                    var resourceAPIModel = mapper.Map<ResourceAPIModel>(resource);
                    resources.Add(resourceAPIModel);
                }
                foreach (var troop in kingdom.Troops)   //list of troops
                {
                    var troopInfo = mapper.Map<TroopInfo>(troop);
                    troops.Add(troopInfo);
                }

                var result = new KingdomDetails()
                {
                    Kingdom = mapper.Map<KingdomAPIModel>(kingdom),
                    Buildings = buildings,
                    Resources = resources,
                    Troops = troops
                };
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e); ;
            }
        }

        public async Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId)?
                                                      .Username == username);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> IsEnoughGoldForAsync(int goldAmount, long buildingTypeId)
        {
            try
            {
                return await Task.FromResult(goldAmount >= DbContext.BuildingTypes.FirstOrDefault
                    (bt => bt.Id == buildingTypeId + 1).GoldCost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<int> GetGoldAmountAsync(long kingdomId)
        {
            try
            {
                var kingdom = await GetByIdAsync(kingdomId);
                return kingdom.Resources.FirstOrDefault(x => x.Type == "gold").Amount;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<ValueTuple<List<LeaderboardKingdomAPIModel>, int, string>> GetKingdomsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await GetAllAsync();
                if (allKingdoms.Count() == 0)
                {
                    //error = "There are no kingdoms in Leaderboard";
                    //status = 404;
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var kingdomsLeaderboard = new List<LeaderboardKingdomAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardKingdomAPIModel>(kingdom);
                    kingdomsLeaderboard.Add(model);
                }
                //error = "ok";
                //status = 200;
                return (kingdomsLeaderboard.OrderByDescending(p => p.Total_points).ToList(), 200, "OK");

            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }
    }
}
