using AutoMapper;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Buildings;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class BuildingService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public BuildingService(IMapper mapper, AuthService authService,
                               KingdomService kingdomService, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            AuthService = authService;
            KingdomService = kingdomService;
            UnitOfWork = unitOfWork;
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

        public BuildingResponse AddBuilding(BuildingRequest request, long kingdomId,
            out int statusCode, out string error)
        {
            try
            {
                var buildingType = UnitOfWork.BuildingTypes.FirstOrDefault //getting required building level 1 information 
                    (bt => bt.Type == request.Type && bt.Level == 1);
                var kingdom = KingdomService.GetById(kingdomId);
                var requiredTownhallLevel = buildingType.RequiredTownhallLevel;

                if ((buildingType.Type == "academy" && kingdom.Buildings.Any(b => b.Type == "academy"))
                   || (buildingType.Type == "ramparts" && kingdom.Buildings.Any(b => b.Type == "ramparts"))
                   || buildingType.Type == "townhall") //checking for buildings that can be built only once 
                {
                    statusCode = 403;
                    error = $"You can have only one {buildingType.Type}!";
                    return new BuildingResponse();
                }

                if (GetTownhallLevel(kingdomId) != requiredTownhallLevel) //checking required townhall level
                {
                    statusCode = 400;
                    error = $"You need to have townhall level {requiredTownhallLevel} to build that!";
                    return new BuildingResponse();
                }

                if (!KingdomService.IsEnoughGoldFor(KingdomService.GetGoldAmount(kingdomId), //checking resources in the kingdom
                    buildingType.Id))
                {
                    statusCode = 400;
                    error = "You don't have enough gold to build that!";
                    return new BuildingResponse();
                }

                var buildingModel = mapper.Map<BuildingModel>(buildingType); //mapping model for creating building
                buildingModel.BuildingTypeId = buildingType.Id;
                buildingModel.KingdomId = kingdom.Id;

                Building building = mapper.Map<Building>(buildingModel); //creating building using reverse mapping
                kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Amount -= buildingType.GoldCost; //charging for creating building
                if (building.Type == "farm")
                {
                    kingdom.Resources.FirstOrDefault(r => r.Type == "food").Generation += 1;
                }
                else if (building.Type == "mine")                                                   //upgrading food/gold generation
                {
                    kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Generation += 1;
                }
                UnitOfWork.Buildings.Add(building);
                UnitOfWork.CompleteAsync();

                statusCode = 200;
                error = string.Empty;
                return mapper.Map<BuildingResponse>(building); //mapping in order to give required response format
            }
            catch
            {
                statusCode = 500;
                error = "Data could not be read";
                return null;
            }
        }

        public int GetTownhallLevel(long kingdomId)
        {
            var kingdom = KingdomService.GetById(kingdomId);
            return kingdom.Buildings.Where(p => p.Type == "townhall").FirstOrDefault().Level;
        }

        public BuildingAPIModel UpgradeBuilding(long kingdomId, long buildingId, out int statusCode,
            out string error)
        {
            try
            {
                var kingdom = KingdomService.GetById(kingdomId);
                var building = kingdom.Buildings.FirstOrDefault(b => b.Id == buildingId);

                if (!KingdomService.IsEnoughGoldFor(KingdomService.GetGoldAmount(kingdomId),
                    building.BuildingTypeId))
                {
                    statusCode = 400;
                    error = "You don't have enough gold to upgrade that!";
                    return new BuildingAPIModel();
                }

                if (building.Type is not "townhall" && GetTownhallLevel(kingdomId) <= building.Level)
                {
                    statusCode = 403;
                    error = "Building's level cannot be higher than the townhall's!";
                    return new BuildingAPIModel();
                }

                if (building.Level == 10)
                {
                    statusCode = 400;
                    error = "This building has already reached max level!";
                    return new BuildingAPIModel();
                }

                var upgradedBuilding = UnitOfWork.BuildingTypes.FirstOrDefault(x => x.Id == building.BuildingTypeId + 1);
                kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Amount -= upgradedBuilding.GoldCost;

                if (upgradedBuilding.Type == "farm")
                {
                    kingdom.Resources.FirstOrDefault(r => r.Type == "food").Generation += 1;
                }
                else if (upgradedBuilding.Type == "mine")
                {
                    kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Generation += 1;
                }

                building.BuildingTypeId = upgradedBuilding.Id;
                building.Level = upgradedBuilding.Level;
                building.Hp = upgradedBuilding.Hp;
                UnitOfWork.CompleteAsync();
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
                var allKingdoms = KingdomService.GetAllKingdoms();
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

        public bool IsBuildingTypeDefined(string type)
        {
            return UnitOfWork.BuildingTypes.Any(bt => bt.Type == type);
        }

        public void AddBasicBuilding(BuildingRequest request, long kingdomId) //similar to AddBuilding method, but modified for player registration
        {
            try
            {
                var buildingType = UnitOfWork.BuildingTypes.FirstOrDefault
                    (bt => bt.Type == request.Type && bt.Level == 1);
                var kingdom = KingdomService.GetById(kingdomId);

                var buildingModel = mapper.Map<BuildingModel>(buildingType);
                buildingModel.BuildingTypeId = buildingType.Id;
                buildingModel.KingdomId = kingdom.Id;

                Building building = mapper.Map<Building>(buildingModel);
                UnitOfWork.Buildings.Add(building);
                UnitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
