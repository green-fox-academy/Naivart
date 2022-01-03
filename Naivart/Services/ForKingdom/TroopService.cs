using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using Naivart.Models.TroopTypes;
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

        public List<TroopInfo> CreateTroops(int goldAmount, string troopType, int troopAmount,
            long kingdomId, out bool isPossibleToCreate)
        {
            var model = TroopFactory(troopType, goldAmount, troopAmount, out int totalCost); //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopInfo>();
            if (model == null)
            {
                isPossibleToCreate = false;
                return resultModel;
            }

            for (int i = 0; i < troopAmount; i++) //create troops number based on troop amount
            {
                var resultTroop = mapper.Map<Troop>(model);
                resultTroop.KingdomId = kingdomId;
                DbContext.Troops.Add(resultTroop);
                DbContext.SaveChanges();
                var infoTroop = mapper.Map<TroopInfo>(resultTroop);
                resultModel.Add(infoTroop);
            }
            var kingdomModel = DbContext.Kingdoms.Where(x => x.Id == kingdomId).Include(x => x.Resources).FirstOrDefault();
            kingdomModel.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= totalCost; //reduce owner gold by total cost
            DbContext.SaveChanges();
            isPossibleToCreate = true;
            return resultModel; //returns list of created troops
        }

        public List<TroopInfo> TroopCreateRequest(CreateTroopRequest input, long kingdomId, 
            string username, out int status, out string result)
        {
            var troopsCreated = new List<TroopInfo>();
            try
            {
                if (!AuthService.IsKingdomOwner(kingdomId, username))
                {
                    status = 401;
                    result = "This kingdom does not belong to authenticated player";
                    return troopsCreated;
                }

                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                troopsCreated = CreateTroops(goldAmount, input.Type, input.Quantity, kingdomId,
                    out bool isPossibleToCreate);

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
            catch
            {
                status = 500;
                result = "Data could not be read";
                return troopsCreated;
            }
        }

        public TroopModel TroopFactory(string troopType, int goldAmount, int troopAmount, out int totalCost)
        {
            switch (troopType) //decide which type is about to be created and gives him proper stats
            {
                case "recruit":
                    TroopModel recruit = new Recruit();
                    totalCost = (recruit.GoldCost * troopAmount);
                    return totalCost <= goldAmount ? recruit : null;
                case "archer":
                    TroopModel archer = new Archer();
                    totalCost = (archer.GoldCost * troopAmount);
                    return totalCost <= goldAmount ? archer : null;
                case "knight":
                    TroopModel knight = new Knight();
                    totalCost = (knight.GoldCost * troopAmount);
                    return totalCost <= goldAmount ? knight : null;
            }
            totalCost = 0;
            return null; //if you dont have money returns null
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
    }
}
