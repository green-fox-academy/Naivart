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
using System.Threading.Tasks;

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

        public async Task<ValueTuple<List<TroopInfo>, bool>> CreateTroopsAsync(int goldAmount, string troopType, int troopAmount, long kingdomId)
        {
            var kingdom = await KingdomService.GetByIdAsync(kingdomId);
            var troop = kingdom.Troops.FirstOrDefault(x => x.TroopType.Type == troopType);
            var result = await TroopFactoryAsync(troopType, goldAmount, troopAmount, troop.TroopType.Level); //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopInfo>();
            if (result.Item1 != null)
            {
                for (int i = 0; i < troopAmount; i++) //create troops number based on troop amount
                {
                    //var resultTroop = mapper.Map<Troop>(createdTroop);
                    //resultTroop.KingdomId = kingdomId;
                    result.Item1.KingdomId = kingdomId;
                    await DbContext.Troops.AddAsync(result.Item1);
                    await DbContext.SaveChangesAsync();
                    var infoTroop = mapper.Map<TroopInfo>(result);
                    resultModel.Add(infoTroop);
                }
                var kingdomModel = await Task.FromResult(DbContext.Kingdoms.Where(x => x.Id == kingdomId)
                                                    .Include(x => x.Resources).FirstOrDefault());
                kingdomModel.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= result.Item2; //reduce owner gold by total cost
                await DbContext.SaveChangesAsync();

                //isPossibleToCreate = true;
                return (resultModel, true); //returns list of created troops and confirmation
            }
            //isPossibleToCreate = false;
            return (resultModel, false);
        }

        public async Task<ValueTuple<List<TroopInfo>, int, string>> TroopCreateRequestAsync(CreateTroopRequest input, long kingdomId, string username)
        {
            var troopsCreated = new List<TroopInfo>();
            try
            {
                if (await AuthService.IsKingdomOwnerAsync(kingdomId, username))
                {
                    int goldAmount = await KingdomService.GetGoldAmountAsync(kingdomId);
                    var response = await CreateTroopsAsync(goldAmount, input.Type, input.Quantity, kingdomId);

                    if (response.Item2)
                    {
                        //status = 200;
                        //result = "ok";
                        return (response.Item1, 200, "OK");
                    }
                    //status = 400;
                    //result = "You don't have enough gold to train all these units!";
                    return (response.Item1, 400, "You don't have enough gold to train all these units!");
                }
                //status = 401;
                //result = "This kingdom does not belong to authenticated player";
                return (troopsCreated, 401, "This kingdom does not belong to authenticated player!");
            }
            catch (Exception)
            {
                //status = 500;
                //result = "Data could not be read";
                return (troopsCreated, 500, "Data could not be read");
            }
        }

        public async Task<ValueTuple<Troop, int>> TroopFactoryAsync(string troopType, int goldAmount, int troopAmount, long troopTypeLevel)
        {
            if (troopTypeLevel == 0) //If there are no troops of its type, set type level to 1 
            {
                troopTypeLevel = 1;
            }
            var troopStats = await Task.FromResult(DbContext.TroopTypes.Where(x => x.Type == troopType 
                                                            && x.Level == troopTypeLevel).FirstOrDefault());

            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id
            };
            var totalCost = (troop.TroopType.GoldCost * troopAmount);
            return totalCost <= goldAmount ? (troop, totalCost) : (null, totalCost);
        }

        public async Task<ValueTuple<List<LeaderboardTroopAPIModel>, int, string>> GetTroopsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await KingdomService.GetAllAsync();
                if (allKingdoms.Count() == 0)
                {
                    //error = "There are no kingdoms in Leaderboard";
                    //status = 404;
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var TroopsLeaderboard = new List<LeaderboardTroopAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardTroopAPIModel>(kingdom);
                    TroopsLeaderboard.Add(model);
                }
                //error = "ok";
                //status = 200;
                return (TroopsLeaderboard.OrderByDescending(p => p.Points).ToList(), 200, "OK");
            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<ValueTuple<int, string>> UpgradeTroopsAsync(long kingdomId, string username, string type)
        {
            try
            {
                if (!await AuthService.IsKingdomOwnerAsync(kingdomId, username))
                {
                    //result = "This kingdom doesn't belong to authenticated player";
                    return (401, "This kingdom doesn't belong to authenticated player");
                }
                int goldAmount = await KingdomService.GetGoldAmountAsync(kingdomId);
                var kingdom = await KingdomService.GetByIdAsync(kingdomId);
                var academy = kingdom.Buildings.Where(t => String.Equals(t.Type, "Academy", 
                    StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //Find the proper type of troop for detect the current level
                var troop = kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, 
                    StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (academy == null) //There is no academy in Kingdom
                {
                    //result = "You have to build Academy first!";
                    return (400, "You have to build Academy first!");
                }
                else if (troop == null) //There are not any troop of this type
                {
                    //result = "You don't have any troop of this type in your army!";
                    return (400, "You don't have any troop of this type in your army!");
                }
                else if (troop.TroopType.Level >= 20) //Max. level reached
                {
                    //result = "Maximum level reached!";
                    return (400, "Maximum level reached!");
                }
                else if (troop.TroopType.Level >= academy.Level) //Academy upgrade required
                {
                    //result = "Upgrade Academy first!";
                    return (400, "Upgrade Academy first!");
                }

                var upgradedStats = await Task.FromResult(DbContext.TroopTypes
                    .Where(t => t.Type == type && t.Level == troop.TroopType.Level + 1)
                    .FirstOrDefault()); //Get stats of particular troop type one level higher than now
                if (goldAmount < upgradedStats.GoldCost) //Lack of gold 
                {
                    //result = "You don't have enough gold to upgrade this type of troops!";
                    return (400, "You don't have enough gold to upgrade this type of troops!");
                }
                await LevelUpAsync(kingdom, type, upgradedStats); //Everything ok - upgrade all troops of its type
                //result = "ok";
                return (200, "OK");
            }
            catch
            {
                //result = "Data couldn't be read";
                return (500, "Data could not be read");
            }
        }

        public async Task LevelUpAsync(Kingdom kingdom, string type, TroopType upgradedStats)
        {
            //Reduce owner gold by upgrade gold cost
            kingdom.Resources.FirstOrDefault(t => t.Type == "gold").Amount -= upgradedStats.GoldCost;

            foreach (Troop troop in kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, 
                StringComparison.CurrentCultureIgnoreCase)).ToList()) //Upgrade all units of its type
            {
                troop.TroopTypeId++;
            }
            DbContext.Update(kingdom);
            await DbContext.SaveChangesAsync();
        }
    }
}
