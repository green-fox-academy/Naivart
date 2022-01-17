using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
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
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public TimeService TimeService { get; set; }
        public TroopService(IMapper mapper, AuthService authService, IUnitOfWork unitOfWork,
                            KingdomService kingdomService, TimeService timeService)
        {
            this.mapper = mapper;
            AuthService = authService;
            KingdomService = kingdomService;
            UnitOfWork = unitOfWork;
            TimeService = timeService;
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
            int troopLevel;
            if (troop is null)
            {
                troopLevel = 1;
            }
            else
            {
                troopLevel = troop.TroopType.Level;
            }
            var createdTroop = await TroopFactoryAsync(troopType, goldAmount, troopAmount, troopLevel); //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopInfo>();
            if (createdTroop.troop != null)
            {
                for (int i = 0; i < troopAmount; i++) //create troops number based on troop amount
                {
                    createdTroop.troop.KingdomId = kingdomId;
                    createdTroop.troop.Status = "recruiting";
                    //time of creating logic
                    createdTroop.troop.StartedAt = TimeService.GetUnixTimeNow();
                    createdTroop.troop.FinishedAt = createdTroop.troop.StartedAt + 600;

                    UnitOfWork.Troops.AddAsync(createdTroop.troop);
                    await UnitOfWork.CompleteAsync();
                    var infoTroop = mapper.Map<TroopInfo>(createdTroop.troop);
                    resultModel.Add(infoTroop);
                    createdTroop.troop = await TroopFactoryAsync(troopType, troopLevel);
                }
                var kingdomModel = await UnitOfWork.Kingdoms.KingdomIncludeResourceByIdAsync(kingdomId);
                kingdomModel.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= createdTroop.totalCost;   //reduce owner gold by total cost
                await UnitOfWork.CompleteAsync();

                return (resultModel, true); //returns list of created troops and confirmation
            }
            return (resultModel, false);
        }

        public async Task<(List<TroopInfo> list, int status, string message)> TroopCreateRequestAsync(CreateTroopRequest input,
            long kingdomId, string username)
        {
            var troopsCreated = new List<TroopInfo>();
            try
            {
                if (await UnitOfWork.Players.IsKingdomOwnerAsync(kingdomId, username))
                {
                    int goldAmount = await KingdomService.GetGoldAmountAsync(kingdomId);
                    var response = await CreateTroopsAsync(goldAmount, input.Type, input.Quantity, kingdomId);

                    if (response.Item2)
                    {
                        return (response.Item1, 200, "OK");
                    }
                    return (response.Item1, 400, "You don't have enough gold to train all these units!");
                }
                return (troopsCreated, 401, "This kingdom does not belong to authenticated player!");
            }
            catch (Exception)
            {
                return (troopsCreated, 500, "Data could not be read");
            }
        }

        public async Task<(Troop troop, int totalCost)> TroopFactoryAsync(string troopType, int goldAmount, int troopAmount, 
            long troopTypeLevel)
        {
            var troopStats = await UnitOfWork.TroopTypes.GetTroopTypeForUpgradeAsync(troopType, troopTypeLevel);
            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id,
                TroopType = troopStats
            };
            var totalCost = (troop.TroopType.GoldCost * troopAmount);
            return totalCost <= goldAmount ? (troop, totalCost) : (null, totalCost);
        }

        public async Task<Troop> TroopFactoryAsync(string troopType, long troopTypeLevel)
        {
            if (troopTypeLevel == 0) //If there are no troops of its type, set type level to 1 
            {
                troopTypeLevel = 1;
            }
            var troopStats = await UnitOfWork.TroopTypes.GetTroopTypeForUpgradeAsync(troopType, troopTypeLevel);

            Troop troop = new Troop()
            {
                TroopTypeId = troopStats.Id,
                TroopType = troopStats
            };
            return troop;
        }

        public async Task<(List<LeaderboardTroopAPIModel> model, int status, string message)> GetTroopsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await UnitOfWork.Kingdoms.GetAllKingdomsAsync();
                if (allKingdoms.Count == 0)
                {
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var TroopsLeaderboard = new List<LeaderboardTroopAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardTroopAPIModel>(kingdom);
                    TroopsLeaderboard.Add(model);
                }
                return (TroopsLeaderboard.OrderByDescending(p => p.Points).ToList(), 200, "OK");
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<(int status, string message)> UpgradeTroopsAsync(long kingdomId, string username, string type)
        {
            try
            {
                if (!await UnitOfWork.Players.IsKingdomOwnerAsync(kingdomId, username))
                {
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
                    return (400, "You have to build Academy first!");
                }
                else if (await UnitOfWork.Battles.IsAttackerInBattle(kingdomId))
                {
                    return (400, "You can't upgrade your troops, if you attack other kingdom!");
                }
                else if (troop == null) //There are not any troop of this type
                {
                    return (400, "You don't have any troop of this type in your army!");
                }
                else if (troop.TroopType.Level >= 20) //Max. level reached
                {
                    return (400, "Maximum level reached!");
                }
                else if (troop.Status == "upgrading")
                {
                    return (400, "Your troops are already upgrading!");
                }
                else if (troop.TroopType.Level >= academy.Level) //Academy upgrade required
                {
                    return (400, "Upgrade Academy first!");
                }
                var upgradedStats = await UnitOfWork.TroopTypes.UpgradeStatsOfTroopAsync(type, troop.TroopType.Level); //Get stats of particular troop type one level higher than now
                if (goldAmount < upgradedStats.GoldCost) //Lack of gold 
                {
                    return (400, "You don't have enough gold to upgrade this type of troops!");
                }
                await LevelUpAsync(kingdom, type, upgradedStats); //Everything ok - upgrade all troops of its type
                return (200, "OK");
            }
            catch
            {
                return (500, "Data could not be read");
            }
        }

        public async Task LevelUpAsync(Kingdom kingdom, string type, TroopType upgradedStats)
        {
            //Reduce owner gold by upgrade gold cost
            kingdom.Resources.FirstOrDefault(t => t.Type == "gold").Amount -= upgradedStats.GoldCost;

            foreach (Troop troop in kingdom.Troops.Where(x => String.Equals(x.TroopType.Type, type, 
                StringComparison.CurrentCultureIgnoreCase)).ToList()) //Edited(timeflow): changed upgrading to waiting for upgrade
            {
                troop.Status = "upgrading";
                troop.StartedAt = TimeService.GetUnixTimeNow();
                troop.FinishedAt = troop.StartedAt + (600 * upgradedStats.Level); //time for upgrade is level * 10mins
            }
            UnitOfWork.Kingdoms.UpdateState(kingdom);
            await UnitOfWork.CompleteAsync();
        }
    }
}
