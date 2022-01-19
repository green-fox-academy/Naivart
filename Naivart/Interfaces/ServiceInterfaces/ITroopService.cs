using Microsoft.AspNetCore.Mvc;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface ITroopService
    {
        List<TroopAPIModel> ListOfTroopsMapping(List<Troop> troops);
        Task<(List<TroopInfo> list, int status, string message)> TroopCreateRequestAsync(CreateTroopRequest input, long kingdomId, string username);
        Task<(Troop troop, int totalCost)> TroopFactoryAsync(string troopType, int goldAmount, int troopAmount,long troopTypeLevel);
        Task<Troop> TroopFactoryAsync(string troopType, long troopTypeLevel);
        Task<(int status, string message)> UpgradeTroopsAsync(long kingdomId, string username, string type);
        Task LevelUpAsync(Kingdom kingdom, string type, TroopType upgradedStats);
        Task<(List<LeaderboardTroopAPIModel> model, int status, string message)> GetTroopsLeaderboardAsync();
        Task<(List<TroopInfo> info, bool isPossibleToCreate)> CreateTroopsAsync(int goldAmount, string troopType, int troopAmount, long kingdomId);
    }
}
