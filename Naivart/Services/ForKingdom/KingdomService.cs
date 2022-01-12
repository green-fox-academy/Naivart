using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Kingdom;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class KingdomService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public AuthService AuthService { get; set; }
        public LoginService LoginService { get; set; }
        public TimeService TimeService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        //public BuildingService BuildingService { get; set; }
        public KingdomService(IMapper mapper, AuthService authService, LoginService loginService, TimeService timeService, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            AuthService = authService;
            LoginService = loginService;
            TimeService = timeService;
            UnitOfWork = unitOfWork;
        }

        public async Task<List<Kingdom>> GetAllAsync()
        {
            return await UnitOfWork.Kingdoms.GetAllKingdomsAsync();
        }

        public async Task<Kingdom> GetByIdAsync(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                return await Task.FromResult(UnitOfWork.Kingdoms.GetAllKingdomsAsync().FirstOrDefault(k => k.Id == id));
            }
            catch
            {
                return kingdom;
            }
        }

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = mapper.Map<KingdomAPIModel>(kingdom);
            var locationAPIModel = mapper.Map<LocationAPIModel>(kingdom.Location);
            kingdomAPIModel.Location = locationAPIModel;
            return kingdomAPIModel;
        }
        public List<KingdomAPIModel> ListOfKingdomsMapping(List<Kingdom> kingdoms)
        {
            var kingdomAPIModels = new List<KingdomAPIModel>();
            if (kingdoms is null)
            {
                return kingdomAPIModels;
            }

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = KingdomMapping(kingdom);
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
        }

        public async Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName)
        {
            return await UnitOfWork.Kingdoms.RenameKingdomAsync(kingdomId,newKingdomName);
        }

        public async Task<ValueTuple<int, string>> RegisterKingdomAsync(KingdomLocationInput input, string usernameToken)
        {
            try
            {
                //status = 400;
                if (!IsCorrectLocationRange(input))
                {
                    return (400, "One or both coordinates are out of valid range (0-99).");
                }
                else if (await IsLocationTakenAsync(input))
                {
                    return (400, "Given coordinates are already taken!");
                }
                else if (await HasAlreadyLocationAsync(input))
                {
                    if (await IsUserKingdomOwnerAsync(input.KingdomId, usernameToken))
                    {
                        await ConnectLocationAsync(input);
                        //status = 200;
                        return (200, "OK");
                    }
                    //status = 401;
                    return (401, "Wrong user authentication!");
                }
                //status = 409;
                return (409, "Your kingdom already have location!");
            }
            catch
            {
                //status = 500;
                return (500, "Data could not be read");
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.CoordinateX >= 0 && input.CoordinateX <= 99 && input.CoordinateY >= 0
                                          && input.CoordinateY <= 99;
        }

        public async Task<bool> IsLocationTakenAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Locations.Any(x => x.CoordinateX == input.CoordinateX && x.CoordinateY == input.CoordinateY));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> HasAlreadyLocationAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Kingdoms.Any(x => x.Id == input.KingdomId && x.LocationId == null));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task ConnectLocationAsync(KingdomLocationInput input)
        {
            try
            {
                var model = new Location() { CoordinateX = input.CoordinateX, CoordinateY = input.CoordinateY };
                await UnitOfWork.Locations.AddAsync(model);
                await UnitOfWork.CompleteAsync();
                long locationId = await Task.FromResult(UnitOfWork.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX && x.CoordinateY == model.CoordinateY).Id);
                await Task.FromResult(UnitOfWork.Kingdoms.FirstOrDefault(x => x.Id == input.KingdomId).LocationId = locationId);
                await UnitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<ValueTuple<KingdomDetails, int, string>> GetKingdomInfoAsync(long kingdomId, string tokenUsername)
        {
            try
            {
                if (await IsUserKingdomOwnerAsync(kingdomId, tokenUsername))
                {
                    //error = "ok";
                    //status = 200;
                    return (await GetAllInfoAboutKingdomAsync(kingdomId), 200, "OK"); //this will use automapper to create an object
                }
                else
                {
                    //error = "This kingdom does not belong to authenticated player";
                    //status = 401;
                    return (null, 401, "This kingdom does not belong to authenticated player");
                }
            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<KingdomDetails> GetAllInfoAboutKingdomAsync(long kingdomId)
        {
            try
            {
                var kingdom = await GetByIdAsync(kingdomId);
                var buildings = new List<BuildingAPIModel>();
                var resources = new List<ResourceAPIModel>();
                var troops = new List<TroopInfo>();

                foreach (var building in kingdom.Buildings)   //list of buildings
                {
                    var buildingAPIModel = mapper.Map<BuildingAPIModel>(building);
                    buildings.Add(buildingAPIModel);
                }
                foreach (var resource in kingdom.Resources)   //list of resources
                {
                    var resourceAPIModel = mapper.Map<ResourceAPIModel>(resource);
                    resources.Add(resourceAPIModel);
                }
                foreach (var troop in kingdom.Troops)   //list of troops
                {
                    var troopInfo = mapper.Map<TroopInfo>(troop);
                    troops.Add(troopInfo);
                }

                var result = new KingdomDetails()
                {
                    Kingdom = mapper.Map<KingdomAPIModel>(kingdom),
                    Buildings = buildings,
                    Resources = resources,
                    Troops = troops
                };
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e); ;
            }
        }

        public async Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Players.FirstOrDefault(x => x.KingdomId == kingdomId)?
                                        .Username == username);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> IsEnoughGoldForAsync(int goldAmount, long buildingTypeId)
        {
            try
            {
                return goldAmount >= await Task.FromResult(UnitOfWork.BuildingTypes.FirstOrDefault
                    (bt => bt.Id == buildingTypeId + 1).GoldCost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<int> GetGoldAmountAsync(long kingdomId)
        {
            try
            {
                var kingdom = await GetByIdAsync(kingdomId);
                return kingdom.Resources.FirstOrDefault(x => x.Type == "gold").Amount;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<ValueTuple<List<LeaderboardKingdomAPIModel>, int, string>> GetKingdomsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await GetAllAsync();
                if (allKingdoms.Count() == 0)
                {
                    //error = "There are no kingdoms in Leaderboard";
                    //status = 404;
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var kingdomsLeaderboard = new List<LeaderboardKingdomAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardKingdomAPIModel>(kingdom);
                    kingdomsLeaderboard.Add(model);
                }
                //error = "ok";
                //status = 200;
                return (kingdomsLeaderboard.OrderByDescending(p => p.Total_points).ToList(), 200, "OK");

            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<ValueTuple<BattleTargetResponse, int, string>> BattleAsync(BattleTargetRequest targetKingdom, long attackerId, string tokenUsername)
        {
            try
            {
                var attacker = await FindPlayerInfoByKingdomIdAsync(attackerId);
                if(!await DoesKingdomExistAsync(targetKingdom.Target.KingdomId))
                {
                    //error = "Target kingdom doesn't exist";
                    //status = 404;
                    return (null, 404, "Target kingdom doesn't exist");
                }
                else if (targetKingdom.Target.KingdomId == attacker.Id)
                {
                    //error = "You can't attack your own kingdom";
                    //status = 405;
                    return (null, 405, "You can't attack your own kingdom!");
                }
                else if (!TroopQuantityCheck(targetKingdom, attacker))
                {
                    //error = "You don't have enough troops";
                    //status = 404;
                    return (null, 404, "You don't have enough troops!");
                }
                else if (await IsUserKingdomOwnerAsync(attackerId, tokenUsername))
                {
                    //error = "ok";
                    //status = 200;
                    return (await StartBattleAsync(targetKingdom, attacker), 200, "OK");
                }
                else
                {
                    //error = "This kingdom does not belong to authenticated player";
                    //status = 401;
                    return (null, 401, "This kingdom does not belong to authenticated player!");
                }
            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<ValueTuple<BattleResultResponse, int, string>> BattleInfoAsync(long battleId, long kingdomId, string tokenUsername)
        {
            try
            {
                if (!await IsUserKingdomOwnerAsync(kingdomId, tokenUsername))
                {
                    //error = "This kingdom does not belong to authenticated player";
                    //status = 401;
                    return (null, 401, "This kingdom does not belong to authenticated player!");
                }
                else if (!await IsKingdomInBattleAsync(battleId, kingdomId))
                {
                    //error = "Kingdom didn't fight in selected battle!";
                    //status = 401;
                    return (null, 401, "Kingdom didn't fight in selected battle!");
                }
                else if (!await DoesBattleExistAsync(battleId))
                {
                    //error = "Battle doesn't exist";
                    //status = 404;
                    return (null, 404, "Battle doesn't exist!");
                }
                else
                {
                    //error = "ok";
                    //status = 200;
                    return (await GetBattleInfoAsync(battleId), 200, "OK");
                }
            }
            catch
            {
                //error = "Data could not be read";
                //status = 500;
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<bool> IsKingdomInBattleAsync(long battleId, long kingdomId)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Battles.Any(x => x.Id == battleId && (x.AttackerId == kingdomId || x.DefenderId == kingdomId)));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> DoesBattleExistAsync(long battleId)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Battles.Any(x => x.Id == battleId));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<BattleResultResponse> GetBattleInfoAsync(long battleId)
        {
            try
            {
                var result = new BattleResultResponse();
                var battle = await Task.FromResult(UnitOfWork.Battles.FirstOrDefault(x => x.Id == battleId));
                var lostTroopsAttacker = await Task.FromResult(UnitOfWork.TroopsLost.Where(x => x.BattleId == battleId &&
                                                                                                                      x.IsAttacker).ToList());
                var lostTroopsDefender = await Task.FromResult(UnitOfWork.TroopsLost.Where(x => x.BattleId == battleId &&
                                                                                                                    !(x.IsAttacker)).ToList());
                var stolenResources = new ResourceStolen() { Food = battle.FoodStolen, Gold = battle.GoldStolen};

                var modelAttackerTroopsLost = new List<LostTroopsAPI>();
                foreach (var item in lostTroopsAttacker)
                {
                    modelAttackerTroopsLost.Add(mapper.Map<LostTroopsAPI>(item));
                }
            
                var modelDefenderTroopsLost = new List<LostTroopsAPI>();
                foreach (var item in lostTroopsAttacker)
                {
                    modelDefenderTroopsLost.Add(mapper.Map<LostTroopsAPI>(item));
                }

                var attackerInfo = new AttackerInfo() { ResourceStolen = stolenResources, TroopsLost = modelAttackerTroopsLost};
                var defenderInfo = new DefenderInfo() { TroopsLost = modelDefenderTroopsLost};

                result = mapper.Map<BattleResultResponse>(battle);
                result.Attacker = attackerInfo;
                result.Defender = defenderInfo;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public async Task<bool> DoesKingdomExistAsync(long kingdomId)
        {
            try
            {
                return await Task.FromResult(UnitOfWork.Kingdoms.Any(x => x.Id == kingdomId));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<BattleTargetResponse> StartBattleAsync(BattleTargetRequest targetKingdom, Kingdom attacker)
        {
            try
            {
                var battle = new Battle()
                {
                    AttackerId = attacker.Id,
                    DefenderId = targetKingdom.Target.KingdomId,
                    BattleType = targetKingdom.BattleType,
                    Status = "on way attack",
                    StartedAt = TimeService.GetUnixTimeNow(),
                    FinishedAt = await CountTravelTimeAsync(targetKingdom.Target.KingdomId, attacker.Id)
                };
                await UnitOfWork.Battles.AddAsync(battle);
                await UnitOfWork.CompleteAsync();

                var attackerTroops = GetTroopLevels(attacker.Troops); 
                var attackingTroops = new List<AttackerTroops>();
                foreach (var troop in targetKingdom.Troops)
                {
                    await UnitOfWork.AttackerTroops.AddAsync(new AttackerTroops()
                    {
                        Type = troop.Type,
                        Quantity = troop.Quantity,
                        Level = attackerTroops.FirstOrDefault(x => x.Type == troop.Type).Level,
                        BattleId = battle.Id
                    });
                    await UnitOfWork.CompleteAsync();
                    
                    //change all attacking troops status to attack that are in town currently
                    for (int i = 0; i < troop.Quantity; i++)
                    {
                        var troopStatus = attacker.Troops.FirstOrDefault(x => x.TroopType.Type == troop.Type && 
                                                                                        x.Status == "town");
                        troopStatus.Status = "attack";
                        UnitOfWork.Troops.UpdateState(troopStatus);
                        await UnitOfWork.CompleteAsync();
                    }
                }
                return new BattleTargetResponse() 
                { 
                    BattleId = battle.Id,
                    ResolutionTime = battle.FinishedAt
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool TroopQuantityCheck(BattleTargetRequest targetKingdom, Kingdom attacker)
        {
            var troopQuantity = GetTroopQuantity(attacker.Troops.Where(x => x.Status == "town").ToList());
            
                   //checks if you have all types of troops needed
            return targetKingdom.Troops.All(x => troopQuantity.Select(x => x.Type).Contains(x.Type)) &&
                   //checks if all types have correct amount of troops        
                   targetKingdom.Troops.All(x => troopQuantity.Any(y => y.Type == x.Type && y.Quantity >= x.Quantity)); 
                    
        }

        public List<TroopBattleInfo> GetTroopQuantity(List<Troop> input)
        {
            var troopQuantity = new List<TroopBattleInfo>();
            foreach (var troop in input)
            {
                if (troopQuantity.Any(x => x.Type == troop.TroopType.Type && x.Level == troop.TroopType.Level))
                {
                    troopQuantity.FirstOrDefault(x => x.Type == troop.TroopType.Type && x.Level == troop.TroopType.Level).Quantity++;
                }
                else
                {
                    troopQuantity.Add(new TroopBattleInfo()
                    {
                        Type = troop.TroopType.Type,
                        Quantity = 1,
                        Level = troop.TroopType.Level
                    });
                }
            }
            return troopQuantity;
        }

        public async Task<Kingdom> FindPlayerInfoByKingdomIdAsync(long kingdomId)
        {
            return await UnitOfWork.Kingdoms.FindPlayerInfoByKingdomIdAsync(kingdomId);
        }

        public async Task<long> CountTravelTimeAsync(long kingdomIdDef, long kingdomIdAtt)
        {
            try
            {
                var locationDef = await Task.FromResult(UnitOfWork.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdDef)
                    .FirstOrDefault().Location);
                var locationAtt = await Task.FromResult(UnitOfWork.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdAtt)
                    .FirstOrDefault().Location);

                //analytic geometry - difference between two points
                double resultA = (locationAtt.CoordinateX - locationDef.CoordinateX) * (locationAtt.CoordinateX - locationDef.CoordinateX)
                                + (locationAtt.CoordinateY - locationDef.CoordinateY) * (locationAtt.CoordinateY - locationDef.CoordinateY);
                double resultB = Math.Sqrt(resultA);
                //travel speed 1 point = 10min => longest distance takes about 24hours
                double resultC = Math.Round((resultB * 600) + TimeService.GetUnixTimeNow());
                return Convert.ToInt64(resultC);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public List<TroopBattleInfo> GetTroopLevels(List<Troop> input)
        {
            var output = new List<TroopBattleInfo>();
            foreach (var troop in input)
            {
                if (!output.Any(x => x.Type == troop.TroopType.Type && x.Level == troop.TroopType.Level))
                {
                    output.Add(new TroopBattleInfo()
                    {
                        Type = troop.TroopType.Type,
                        Level = troop.TroopType.Level
                    });
                }
            }
            return output;
        }
    }
}
