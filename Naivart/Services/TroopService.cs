﻿using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;
using Naivart.Models.TroopTypes;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Naivart.Services
{
    public class TroopService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TroopService(ApplicationDbContext dbContext, AuthService authService, KingdomService kingdomService, IMapper mapper)
        {
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
            this.mapper = mapper;
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
            var model = TroopFactory(troopType, goldAmount, troopAmount, out int totalCost);    //get troop stats based on type, if no golds returns null
            var resultModel = new List<TroopsInfo>();
            if (model != null)
            {
                for (int i = 0; i < troopAmount; i++)   //create troops number based on troop amount
                {
                    var resultTroop = mapper.Map<Troop>(model);
                    resultTroop.KingdomId = kingdomId;
                    DbContext.Troops.Add(resultTroop);
                    DbContext.SaveChanges();
                    var infoTroop = mapper.Map<TroopsInfo>(resultTroop);
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

        public TroopModel TroopFactory(string troopType, int goldAmount, int troopAmount, out int totalCost)
        {
            switch (troopType)  //decide which type is about to be created and gives him proper stats
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
            return null;    //if you dont have money returns null
        }

        public void UpgradeTroops(long kingdomId, string username, string type, out int statusCode, out string result)
        {
            try
            {
                if (AuthService.IsKingdomOwner(kingdomId, username))
                {
                    int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                    var troopsLevelUp = TroopsLevelUp(kingdomId, type, goldAmount);
                    if (troopsLevelUp)
                    {
                        statusCode = 200;
                        result = "ok";
                    }
                    else if()
                    statusCode = 400;
                    result = "You don't have enough gold to train all these units!";
                }
                statusCode = 401;
                result = "This kingdom does not belong to authenticated player";
            }
            catch (Exception)
            {
                statusCode = 500;
                result = "Data could not be read";
            }
        }

        public bool TroopsLevelUp(long kingdomId, string troopType, int goldAmount)
        {

        }
    }
}
