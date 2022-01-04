using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.BuildingTypes;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class BuildingService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public BuildingService(IMapper mapper, ApplicationDbContext dbContext, AuthService authService,
                               KingdomService kingdomService, PlayerService playerService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
            PlayerService = playerService;
        }

        public List<BuildingAPIModel> ListOfBuildingsMapping(List<Building> buildings)
        {
            var buildingAPIModels = new List<BuildingAPIModel>();
            if (buildings is null)
            {
                return buildingAPIModels;
            }

            foreach (var building in buildings)
            {
                var buildingAPIModel = mapper.Map<BuildingAPIModel>(building);
                buildingAPIModels.Add(buildingAPIModel);
            }
            return buildingAPIModels;
        }

        public AddBuildingResponse AddBuilding(AddBuildingRequest input, long kingdomId,
            string username, out int status)
        {
            var response = new AddBuildingResponse();
            try
            {
                if (!AuthService.IsKingdomOwner(kingdomId, username))
                {
                    status = 401;
                    return response;
                }
                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                int townHallLevel = GetTownhallLevel(kingdomId);
                response = CreateBuilding(goldAmount, input.Type, townHallLevel, kingdomId,
                    out bool isPossibleToCreate);
                status = isPossibleToCreate ? 200 : 400;
                return response;
            }
            catch
            {
                status = 500;
                return response;
            }
        }
        public BuildingModel BuildingCreation(int goldAmount, string buildingtype, int townHallLevel)
        {
            switch (buildingtype)
            {
                case "academy":
                    BuildingModel academy = new Academy();
                    if (academy.GoldCost <= goldAmount && academy.RequestTownhallLevel <= townHallLevel)
                    {
                        return academy;
                    }
                    return null;
                case "barracks":
                    BuildingModel barracks = new Barracks();
                    if (barracks.GoldCost <= goldAmount && barracks.RequestTownhallLevel <= townHallLevel)
                    {
                        return barracks;
                    }
                    return null;
                case "farm":
                    BuildingModel farm = new Farm();
                    if (farm.GoldCost <= goldAmount && farm.RequestTownhallLevel <= townHallLevel)
                    {
                        return farm;
                    }
                    return null;
                case "mine":
                    BuildingModel mine = new Mine();
                    if (mine.GoldCost <= goldAmount && mine.RequestTownhallLevel <= townHallLevel)
                    {
                        return mine;
                    }
                    return null;
                case "walls":
                    BuildingModel walls = new Walls();
                    if (walls.GoldCost <= goldAmount && walls.RequestTownhallLevel <= townHallLevel)
                    {
                        return walls;
                    }
                    return null;
            }
            return null;
        }

        public int GetTownhallLevel(long kingdomId)
        {
            var kingdom = KingdomService.GetById(kingdomId);
            return kingdom.Buildings.Where(p => p.Type == "townhall").FirstOrDefault().Level;
        }

        public AddBuildingResponse CreateBuilding(int goldAmount, string buildingType, int townHallLevel,
            long kingdomId, out bool isPossibleToCreate)
        {
            var buildingModel = BuildingCreation(goldAmount, buildingType, townHallLevel);
            var response = new AddBuildingResponse();
            if (buildingModel == null)
            {
                isPossibleToCreate = false;
                return response;
            }

            var building = mapper.Map<Building>(buildingModel);
            building.KingdomId = kingdomId;
            DbContext.Buildings.Add(building);
            DbContext.SaveChanges();
            response = mapper.Map<AddBuildingResponse>(building);
            var kingdom = KingdomService.GetById(kingdomId);
            kingdom.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= buildingModel.GoldCost;
            DbContext.SaveChanges();
            isPossibleToCreate = true;
            return response;
        }

        public BuildingAPIModel UpgradeBuilding(long kingdomId, long buildingId, string operation,
            out int statusCode, out string error)
        {
            try
            {
                if (!KingdomService.IsEnoughGoldFor(KingdomService.GetGoldAmount(kingdomId), operation))
                {
                    statusCode = 400;
                    error = "You don't have enough gold to upgrade that!";
                    return new BuildingAPIModel();
                }

                var building = KingdomService.GetById(kingdomId).Buildings
                    .FirstOrDefault(b => b.Id == buildingId);
                building.Level += 1;
                DbContext.SaveChanges();
                statusCode = 200;
                error = string.Empty;
                return mapper.Map<BuildingAPIModel>(building);
            }
            catch
            {
                statusCode = 500;
                error = "Data could not be read";
                return null;
            }
        }

        public List<LeaderboardBuildingAPIModel> GetBuildingsLeaderboard(out int status, out string error)
        {
            try
            {
                var allKingdoms = KingdomService.GetAll();
                if (!allKingdoms.Any())
                {
                    error = "There are no kingdoms in Leaderboard";
                    status = 404;
                    return null;
                }

                var BuildingsLeaderboard = new List<LeaderboardBuildingAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardBuildingAPIModel>(kingdom);
                    BuildingsLeaderboard.Add(model);
                }
                error = "ok";
                status = 200;
                return BuildingsLeaderboard.OrderByDescending(p => p.Points).ToList();
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
