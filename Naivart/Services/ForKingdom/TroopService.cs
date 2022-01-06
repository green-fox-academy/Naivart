using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class TroopService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TroopService(IMapper mapper, ApplicationDbContext dbContext, AuthService authService,
                            KingdomService kingdomService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
        }

        public List<TroopAPIModel> ListOfTroopsMapping(List<Troop> troops)
        {
            var troopAPIModels = new List<TroopAPIModel>();
            if (troops is null)
            {
                return troopAPIModels;
            }

            foreach (var troop in troops)
            {
                var troopAPIModel = mapper.Map<TroopAPIModel>(troop);
                troopAPIModels.Add(troopAPIModel);
            }
            return troopAPIModels;
        }

        public List<TroopInfo> CreateTroops(int goldAmount, string troopType, int troopAmount, long kingdomId, out bool isPossibleToCreate)
        {
            var troop = KingdomService.GetById(kingdomId).Troops.FirstOrDefault(x => x.TroopType.Type == troopType);
            int troopLevel;
            if (troop is null)
            {
                troopLevel = 1;
            }
            else
            {
                troopLevel = troop.TroopType.Level;
            }
            var createdTroop = TroopFactory(troopType, goldAmount, troopAmount, troopLevel, out int totalCost);    //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopInfo>();
            if (createdTroop != null)
            {
                for (int i = 0; i < troopAmount; i++)   //create troops number based on troop amount
                {
                    //var resultTroop = mapper.Map<Troop>(createdTroop);
                    //resultTroop.KingdomId = kingdomId;
                    createdTroop.KingdomId = kingdomId;
                    DbContext.Troops.Add(createdTroop);
                    DbContext.SaveChanges();
                    var infoTroop = mapper.Map<TroopInfo>(createdTroop);
                    resultModel.Add(infoTroop);
                    createdTroop = TroopFactory(troopType, troopLevel);
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

        public List<TroopInfo> TroopCreateRequest(CreateTroopRequest input, long kingdomId, string username, out int status, out string result)
        {
            var troopsCreated = new List<TroopInfo>();
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
            var troopStats = DbContext.TroopTypes.Where(x => x.Type == troopType && x.Level == troopTypeLevel).FirstOrDefault();

            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id,
                TroopType = troopStats
            };
            totalCost = (troop.TroopType.GoldCost * troopAmount);
            return totalCost <= goldAmount ? troop : null;
        }

        public Troop TroopFactory(string troopType, long troopTypeLevel)
        {
            if (troopTypeLevel == 0) //If there are no troops of its type, set type level to 1 
            {
                troopTypeLevel = 1;
            }
            var troopStats = DbContext.TroopTypes.Where(x => x.Type == troopType && x.Level == troopTypeLevel).FirstOrDefault();

            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id,
                TroopType = troopStats
            };
            return troop;
        }

        public List<LeaderboardTroopAPIModel> GetTroopsLeaderboard(out int status, out string error)
        {
            try
            {
                var allKingdoms = KingdomService.GetAll();
                if (allKingdoms.Count() == 0)
                {
                    error = "There are no kingdoms in Leaderboard";
                    status = 404;
                    return null;
                }

                var TroopsLeaderboard = new List<LeaderboardTroopAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardTroopAPIModel>(kingdom);
                    TroopsLeaderboard.Add(model);
                }
                error = "ok";
                status = 200;
                return TroopsLeaderboard.OrderByDescending(p => p.Points).ToList();
            }
            catch
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
                if (!AuthService.IsKingdomOwner(kingdomId, username))
                {
                    result = "This kingdom doesn't belong to authenticated player";
                    return statusCode = 401;
                }
                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                var kingdom = KingdomService.GetById(kingdomId);
                var academy = kingdom.Buildings.Where(t => String.Equals(t.Type, "Academy", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //Find the proper type of troop for detect the current level
                var troop = kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (academy == null) //There is no academy in Kingdom
                {
                    result = "You have to build Academy first!";
                    return statusCode = 400;
                }
                else if (troop == null) //There are not any troop of this type
                {
                    result = "You don't have any troop of this type in your army!";
                    return statusCode = 400;
                }
                else if (troop.TroopType.Level >= 20) //Max. level reached
                {
                    result = "Maximum level reached!";
                    return statusCode = 400;
                }
                else if (troop.TroopType.Level >= academy.Level) //Academy upgrade required
                {
                    result = "Upgrade Academy first!";
                    return statusCode = 400;
                }

                var upgradedStats = DbContext.TroopTypes.Where(t => t.Type == type && t.Level == troop.TroopType.Level + 1).FirstOrDefault(); //Get stats of particular troop type one level higher than now
                if (goldAmount < upgradedStats.GoldCost) //Lack of gold 
                {
                    result = "You don't have enough gold to upgrade this type of troops!";
                    return statusCode = 400;
                }
                LevelUp(kingdom, type, upgradedStats); //Everything ok - upgrade all troops of its type
                result = "ok";
                return statusCode = 200;
            }
            catch (Exception)
            {
                result = "Data couldn't be read";
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
