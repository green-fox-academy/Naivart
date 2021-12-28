using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Naivart.Models.APIModels.Leaderboards;

namespace Naivart.Services
{
    public class TroopService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public BuildingService BuildingService { get; set; }
        public TroopService(ApplicationDbContext dbContext, AuthService authService, KingdomService kingdomService, IMapper mapper, BuildingService buildingService)
        {
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
            this.mapper = mapper;
            BuildingService = buildingService;
        }

        public List<TroopAPIModel> ListOfTroopsMapping(List<Troop> troops)
        {
            var TroopsAPIModels = new List<TroopAPIModel>();

            foreach (var troop in troops)
            {
                var TroopsAPIModel = mapper.Map<TroopAPIModel>(troop);
                TroopsAPIModels.Add(TroopsAPIModel);
            }
            return TroopsAPIModels;
        }


        public List<TroopsInfo> CreateTroops(int goldAmount, string troopType, int troopAmount, long kingdomId, out bool isPossibleToCreate)
        {
            var troop = KingdomService.GetById(kingdomId).Troops.FirstOrDefault(x => x.TroopType.Type == troopType);
            var createdTroop = TroopFactory(troopType, goldAmount, troopAmount, troop.TroopType.Level, out int totalCost);    //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopsInfo>();
            if (createdTroop != null)
            {
                for (int i = 0; i < troopAmount; i++)   //create troops number based on troop amount
                {
                    //var resultTroop = mapper.Map<Troop>(createdTroop);
                    //resultTroop.KingdomId = kingdomId;
                    createdTroop.KingdomId = kingdomId;
                    DbContext.Troops.Add(createdTroop);
                    DbContext.SaveChanges();
                    var infoTroop = mapper.Map<TroopsInfo>(createdTroop);
                    resultModel.Add(infoTroop);
                }
                var kingdomModel = DbContext.Kingdoms.Where(x => x.Id == kingdomId).Include(x => x.Resources).FirstOrDefault();
                kingdomModel.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= totalCost;   //reduce owner gold by total cost
                DbContext.SaveChanges();

                isPossibleToCreate = true;
                return resultModel; //returns list of created troops
            }
            isPossibleToCreate = false;
            return resultModel;
        }

        public List<TroopsInfo> TroopCreateRequest(CreateTroopAPIRequest input, long kingdomId, string username, out int status, out string result)
        {
            var troopsCreated = new List<TroopsInfo>();
            try
            {
                if (AuthService.IsKingdomOwner(kingdomId, username))
                {
                    int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                    troopsCreated = CreateTroops(goldAmount, input.Type, input.Quantity, kingdomId, out bool isPossibleToCreate);

                    if (isPossibleToCreate)
                    {
                        status = 200;
                        result = "ok";
                        return troopsCreated;
                    }
                    status = 400;
                    result = "You don't have enough gold to train all these units!";
                    return troopsCreated;
                }
                status = 401;
                result = "This kingdom does not belong to authenticated player";
                return troopsCreated;
            }
            catch (Exception)
            {
                status = 500;
                result = "Data could not be read";
                return troopsCreated;
            }
        }

        public Troop TroopFactory(string troopType, int goldAmount, int troopAmount, long troopTypeLevel, out int totalCost)
        {
            if (troopTypeLevel == 0) //If there are no troops of its type, set type level to 1 
            {
                troopTypeLevel = 1;
            }
            var troopStats = DbContext.TroopTypes.Where(x => x.Type == troopType && x.Level == troopTypeLevel).FirstOrDefault();

            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id
            };
            totalCost = (troop.TroopType.GoldCost * troopAmount);
            return totalCost <= goldAmount ? troop : null;
        }

        public List<LeaderboardTroopAPIModel> GetTroopsLeaderboard(out int status, out string error)
        {
            try
            {
                var AllKingdoms = DbContext.Kingdoms.Include(k => k.Player)
                                                    .Include(k => k.Buildings)
                                                    .Include(k => k.Troops)
                                                    .ThenInclude(k=>k.TroopType)
                                                    .ToList();
                if (AllKingdoms.Count() > 0)
                {
                    var TroopsLeaderboard = new List<LeaderboardTroopAPIModel>();
                    foreach (var kingdom in AllKingdoms)
                    {
                        var buildingMapper = mapper.Map<LeaderboardTroopAPIModel>(kingdom);
                        TroopsLeaderboard.Add(buildingMapper);
                    }
                    error = "ok";
                    status = 200;
                    TroopsLeaderboard = TroopsLeaderboard.OrderByDescending(p => p.points).ToList();
                    return TroopsLeaderboard;
                }
                else
                {
                    error = "There are no kingdoms in Leaderboard";
                    status = 404;
                    return null;
                }
            }
            catch (Exception)
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }

        public int UpgradeTroops(long kingdomId, string username, string type, out string result, out int statusCode)
        {
            try
            {
                if (AuthService.IsKingdomOwner(kingdomId, username))
                {
                    int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                    var kingdom = KingdomService.GetById(kingdomId);
                    var academy = kingdom.Buildings.Where(t => String.Equals(t.Type, "Academy", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    //Find the right type of troop for detect the current level
                    var troop = kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                    if (academy == null) //There is no academy in Kingdom
                    {
                        result = "You have to build Academy first.";
                        return statusCode = 400;
                    }
                    else if (troop == null) //There are not any troop of this type
                    {
                        result = $"You dont have any {type} in your army to upgrade.";
                        return statusCode = 400;
                    }
                    else if (troop.TroopType.Level >= academy.Level) //Academy upgrade required
                    {
                        result = "Upgrade Academy first.";
                        return statusCode = 400;
                    }
                    else if (troop.TroopType.Level >= 20) //Max. level reached
                    {
                        result = $"{type} reached maximum level.";
                        return statusCode = 400;
                    }

                    var upgradedStats = DbContext.TroopTypes.Where(t => t.Type == type && t.Level == troop.TroopType.Level + 1).FirstOrDefault();
                    if (goldAmount < upgradedStats.GoldCost) //Lack of gold 
                    {
                        result = "You don't have enough gold to upgrade this type of troops!";
                        return statusCode = 400;
                    }
                    LevelUp(kingdom, type,upgradedStats); //Everything ok - upgrade all troops of its type
                    result = "ok";
                    return statusCode = 200;
                }
                result = "This kingdom does not belong to authenticated player";
                return statusCode = 401;
            }
            catch (Exception)
            {
                result = "Data could not be read";
                return statusCode = 500;
            }
        }

        public void LevelUp(Kingdom kingdom, string type, TroopType upgradedStats)
        {
            //Reduce owner gold by upgrade gold cost
            kingdom.Resources.FirstOrDefault(t => t.Type == "gold")
                .Amount -= upgradedStats.GoldCost;

            foreach (Troop troop in kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, StringComparison.CurrentCultureIgnoreCase)).ToList()) //Upgrade all units of its type
            {
                troop.TroopTypeId++;
            }
            DbContext.Update(kingdom);
            DbContext.SaveChanges();
        }
    }
}
