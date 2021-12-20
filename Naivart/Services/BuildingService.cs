using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.BuildingTypes;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class BuildingService
    {
        private readonly IMapper mapper;
        private ApplicationDbContext DbContext { get; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public AuthService AuthService { get; set; }
        public BuildingService(IMapper mapper, ApplicationDbContext dbContext, KingdomService kingdomService,PlayerService playerService, AuthService  authService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            PlayerService = playerService;
            KingdomService = kingdomService;
            AuthService = authService;
        }

        public List<BuildingsForResponse> ListOfBuildingMapping(List<Building> buildings)
        {
            var buildingForResponses = new List<BuildingsForResponse>();

            if (buildings is null)
            {
                return buildingForResponses;
            }

            foreach (var building in buildings)
            {
                var buildingForResponse = mapper.Map<BuildingsForResponse>(building);
                buildingForResponses.Add(buildingForResponse);
            }
            return buildingForResponses;
        }

        public AddBuildingForResponse AddBuilding(AddBuildingResponse input, long kingdomId, string username, out int status)
        {
            var addBuilding = new AddBuildingForResponse();
            try
            {
                if (AuthService.IsKingdomOwner(kingdomId, username))
                {
                    int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                    int townHallLevel = GetTownhallLevel(kingdomId);
                    addBuilding = CreateBuilding(goldAmount, input.Type, townHallLevel, kingdomId,out bool isPossibleToCreate);

                    if (isPossibleToCreate)
                    {
                        status = 200;
                        return addBuilding;
                    }

                    status = 400;
                    return addBuilding;
                }
                status = 401;
                return addBuilding;
            }
            catch (Exception)
            {
                status = 500;
                return addBuilding;
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
            var buildings = DbContext.Kingdoms.Where(p => p.Id == kingdomId).Include(p => p.Buildings).FirstOrDefault();
            return buildings.Buildings.Where(p => p.Type == "townhall").FirstOrDefault().Level;
        }
        public AddBuildingForResponse CreateBuilding(int goldAmount, string buildingType, int townHallLevel, long kingdomId, out bool isPossibleToCreate)
        {
            var model = BuildingCreation(goldAmount, buildingType, townHallLevel);
            var resultModel = new AddBuildingForResponse();
            if (model != null)
            {
                var resultBuilding = mapper.Map<Building>(model);
                resultBuilding.KingdomId = kingdomId;
                DbContext.Buildings.Add(resultBuilding);
                DbContext.SaveChanges();
                resultModel = mapper.Map<AddBuildingForResponse>(resultBuilding);
                var kingdomModel = DbContext.Kingdoms.Where(x => x.Id == kingdomId).Include(x => x.Resources).FirstOrDefault();
                kingdomModel.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= model.GoldCost;
                DbContext.SaveChanges();

                isPossibleToCreate = true;
                return resultModel;
            }
            isPossibleToCreate = false;
            return resultModel;
        }

        public BuildingsForResponse UpgradeBuilding (long kingdomId, long buildingId, string operation, out int statusCode, out string error)
        {
            try
            {
                if (!KingdomService.IsEnoughGoldFor(KingdomService.GetGoldAmount(kingdomId), operation))
                {
                    statusCode = 400;
                    error = "You don't have enough gold to upgrade that!";
                    return new BuildingsForResponse();
                }

                var building = KingdomService.GetByIdWithBuilding(kingdomId).
                    Buildings.Where(b => b.Id == buildingId).FirstOrDefault();
                building.Level += 1;
                DbContext.SaveChanges();
                statusCode = 200;
                error = string.Empty;
                return mapper.Map<BuildingsForResponse>(building);
            }
            catch (Exception)
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
                var AllKingdoms = DbContext.Kingdoms.Include(k => k.Player)
                                                    .Include(k => k.Buildings)
                                                    .ToList();
                if (AllKingdoms.Any())
                {
                    var BuildingsLeaderboard = new List<LeaderboardBuildingAPIModel>();
                    foreach (var kingdom in AllKingdoms)
                    {
                        var buildingMapper = mapper.Map<LeaderboardBuildingAPIModel>(kingdom);
                        BuildingsLeaderboard.Add(buildingMapper);
                    }
                    error = "ok";
                    status = 200;
                    BuildingsLeaderboard = BuildingsLeaderboard.OrderByDescending(p => p.points).ToList();
                    return BuildingsLeaderboard;
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
    }
}
