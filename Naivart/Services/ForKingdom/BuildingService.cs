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
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class BuildingService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private IUnitOfWork _unitOfWork { get; set; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TimeService TimeService { get; set; }
        public BuildingService(IMapper mapper, IUnitOfWork unitOfWork, AuthService authService, KingdomService kingdomService,
                               TimeService timeService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            AuthService = authService;
            KingdomService = kingdomService;
            TimeService = timeService;
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
                buildingAPIModels.Add(_mapper.Map<BuildingAPIModel>(building));
            }
            return buildingAPIModels;
        }

        public async Task<(BuildingResponse model, int status, string message)> AddBuildingAsync(BuildingRequest request, long kingdomId)
        {
            try
            {
                var buildingType = await Task.FromResult(_unitOfWork.BuildingTypes.FirstOrDefault //getting required building level 1 information 
                    (bt => bt.Type == request.Type && bt.Level == 1));
                var kingdom = await KingdomService.GetByIdAsync(kingdomId);
                var requiredTownhallLevel = buildingType.RequiredTownhallLevel;

                if ((buildingType.Type == "academy" && kingdom.Buildings.Any(b => b.Type == "academy"))
                   || (buildingType.Type == "ramparts" && kingdom.Buildings.Any(b => b.Type == "ramparts"))
                   || buildingType.Type == "townhall") //checking for buildings that can be built only once 
                {
                    return (new BuildingResponse(), 403, $"You can have only one {buildingType.Type}!");
                }

                if ((buildingType.Type == "farm" && (kingdom.Buildings.Where(x => x.Type == "farm").Count() >=5 ))
                     || (buildingType.Type == "mine" && (kingdom.Buildings.Where(x => x.Type == "mine").Count() >= 5))) //checking for buildings that can be 5x max 
                {
                    return (new BuildingResponse(), 403, $"You can have only five {buildingType.Type}!");
                }

                if (await GetTownhallLevelAsync(kingdomId) < requiredTownhallLevel) //checking required townhall level
                {
                    return (new BuildingResponse(), 400, $"You need to have townhall level {requiredTownhallLevel} first!");
                }

                if (!await _unitOfWork.BuildingTypes.IsEnoughGoldForAsync(await KingdomService.GetGoldAmountAsync(kingdomId), //checking resources in the kingdom
                    buildingType.Type, buildingType.Level))
                {
                    return (new BuildingResponse(), 400, "You don't have enough gold to build that!");
                }

                var buildingModel = _mapper.Map<BuildingModel>(buildingType); //mapping model for creating building
                buildingModel.BuildingTypeId = buildingType.Id;
                buildingModel.KingdomId = kingdom.Id;

                var building = _mapper.Map<Building>(buildingModel); //creating building using reverse mapping
                kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Amount -= buildingType.GoldCost; //charging for creating building

                building.StartedAt = TimeService.GetUnixTimeNow();
                building.FinishedAt = building.StartedAt + 600;
                building.Status = "creating";
                _unitOfWork.Buildings.AddAsync(building);
                await _unitOfWork.CompleteAsync();

                return (_mapper.Map<BuildingResponse>(building), 200, string.Empty); //mapping in order to give required response format
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<int> GetTownhallLevelAsync(long kingdomId)
        {
            var kingdom = await KingdomService.GetByIdAsync(kingdomId);
            return kingdom.Buildings.FirstOrDefault(p => p.Type == "townhall").Level;
        }

        public async Task<(BuildingAPIModel model, int status, string message)> UpgradeBuildingAsync(long kingdomId, long buildingId)
        {
            try
            {
                var kingdom = await KingdomService.GetByIdAsync(kingdomId);
                var building = kingdom.Buildings.FirstOrDefault(b => b.Id == buildingId);

                if (!await _unitOfWork.BuildingTypes.IsEnoughGoldForAsync(await KingdomService.GetGoldAmountAsync(kingdomId),
                    building.Type, building.Level + 1))
                {
                    return (new BuildingAPIModel(), 400, "You don't have enough gold to upgrade that!");
                }

                if (building.Type is not "townhall" && await GetTownhallLevelAsync(kingdomId) <= building.Level)
                {
                    return (new BuildingAPIModel(), 403, "Building's level cannot be higher than the townhall's!");
                }

                if (building.Level == 10)
                {
                    return (new BuildingAPIModel(), 400, "This building has already reached max level!");
                }
                if (building.Status == "upgrading")
                {
                    return (new BuildingAPIModel(), 400, "This building is already upgrading!");
                }
                if (building.Status == "creating")
                {
                    return (new BuildingAPIModel(), 400, "You must finish creating this building first!");
                }

                var upgradedBuilding = await _unitOfWork.BuildingTypes.GetBuildingTypeAsync(building.Type,
                                                  building.Level + 1);
                kingdom.Resources.FirstOrDefault(r => r.Type == "gold").Amount -= upgradedBuilding.GoldCost;

                building.Status = "upgrading";
                building.StartedAt = TimeService.GetUnixTimeNow();
                building.FinishedAt = building.StartedAt + (600 * upgradedBuilding.Level);
                await _unitOfWork.CompleteAsync();
                return (_mapper.Map<BuildingAPIModel>(building), 200, string.Empty);
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<(List<LeaderboardBuildingAPIModel> model, int status, string message)> GetBuildingsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await _unitOfWork.Kingdoms.GetAllKingdomsAsync();
                if (!allKingdoms.Any())
                {
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var buildingsLeaderboard = new List<LeaderboardBuildingAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = _mapper.Map<LeaderboardBuildingAPIModel>(kingdom);
                    buildingsLeaderboard.Add(model);
                }
                return (buildingsLeaderboard.OrderByDescending(p => p.Points).ToList(), 200, "OK");
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<bool> IsBuildingTypeDefinedAsync(string type)
        {
            return await Task.FromResult(_unitOfWork.BuildingTypes.Any(bt => bt.Type == type));
        }

        public async Task AddBasicBuildingAsync(BuildingRequest request, long kingdomId) //similar to AddBuilding method, but modified for player registration
        {
            try
            {
                var buildingType = await Task.FromResult(_unitOfWork.BuildingTypes.FirstOrDefault
                    (bt => bt.Type == request.Type && bt.Level == 1));
                var kingdom = await KingdomService.GetByIdAsync(kingdomId);

                var buildingModel = _mapper.Map<BuildingModel>(buildingType);
                buildingModel.BuildingTypeId = buildingType.Id;
                buildingModel.KingdomId = kingdom.Id;

                var building = _mapper.Map<Building>(buildingModel);
                building.Status = "done";
                _unitOfWork.Buildings.AddAsync(building);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
