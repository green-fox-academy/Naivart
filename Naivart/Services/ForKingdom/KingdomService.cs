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

        public List<Kingdom> GetAllKingdoms()
        {
            return UnitOfWork.Kingdoms.GetAllKingdoms();
        }

        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                return kingdom = UnitOfWork.Kingdoms.GetAllKingdoms().FirstOrDefault(k => k.Id == id);
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

        public Kingdom RenameKingdom(long kingdomId, string newKingdomName)
        {
            return UnitOfWork.Kingdoms.RenameKingdom(kingdomId,newKingdomName);
        }

        public string RegisterKingdom(KingdomLocationInput input, string usernameToken, out int status)
        {
            try
            {
                status = 400;
                if (!IsCorrectLocationRange(input))
                {
                    return "One or both coordinates are out of valid range (0-99).";
                }
                else if (IsLocationTaken(input))
                {
                    return "Given coordinates are already taken!";
                }
                else if (HasAlreadyLocation(input))
                {
                    if (IsUserKingdomOwner(input.KingdomId, usernameToken))
                    {
                        ConnectLocation(input);
                        status = 200;
                        return "ok";
                    }
                    status = 401;
                    return "Wrong user authentication!";
                }
                status = 409;
                return "Your kingdom already have location!";
            }
            catch
            {
                status = 500;
                return "Data could not be read";
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.CoordinateX >= 0 && input.CoordinateX <= 99 && input.CoordinateY >= 0
                                          && input.CoordinateY <= 99;
        }

        public bool IsLocationTaken(KingdomLocationInput input)
        {
            try
            {
                return UnitOfWork.Locations.Any(x => x.CoordinateX == input.CoordinateX && x.CoordinateY == input.CoordinateY);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool HasAlreadyLocation(KingdomLocationInput input)
        {
            try
            {
                return UnitOfWork.Kingdoms.Any(x => x.Id == input.KingdomId && x.LocationId == null);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public void ConnectLocation(KingdomLocationInput input)
        {
            try
            {
                var model = new Location() { CoordinateX = input.CoordinateX, CoordinateY = input.CoordinateY };
                UnitOfWork.Locations.Add(model);
                UnitOfWork.CompleteAsync();
                long locationId = UnitOfWork.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX && x.CoordinateY == model.CoordinateY).Id;
                UnitOfWork.Kingdoms.FirstOrDefault(x => x.Id == input.KingdomId).LocationId = locationId;
                UnitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public KingdomDetails GetKingdomInfo(long kingdomId, string tokenUsername,
            out int status, out string error)
        {
            try
            {
                if (IsUserKingdomOwner(kingdomId, tokenUsername))
                {
                    error = "ok";
                    status = 200;
                    return GetAllInfoAboutKingdom(kingdomId); //this will use automapper to create an object
                }
                else
                {
                    error = "This kingdom does not belong to authenticated player";
                    status = 401;
                    return null;
                }
            }
            catch
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }

        public KingdomDetails GetAllInfoAboutKingdom(long kingdomId)
        {
            try
            {
                var kingdom = GetById(kingdomId);
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

        public bool IsUserKingdomOwner(long kingdomId, string username)
        {
            try
            {
                return UnitOfWork.Players.FirstOrDefault(x => x.KingdomId == kingdomId)?
                                        .Username == username;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool IsEnoughGoldFor(int goldAmount, long buildingTypeId)
        {
            try
            {
                return goldAmount >= UnitOfWork.BuildingTypes.FirstOrDefault
                    (bt => bt.Id == buildingTypeId + 1).GoldCost;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public int GetGoldAmount(long kingdomId)
        {
            try
            {
                var kingdom = GetById(kingdomId);
                return kingdom.Resources.FirstOrDefault(x => x.Type == "gold").Amount;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public List<LeaderboardKingdomAPIModel> GetKingdomsLeaderboard(out int status, out string error)
        {
            try
            {
                var allKingdoms = GetAllKingdoms();
                if (allKingdoms.Count() == 0)
                {
                    error = "There are no kingdoms in Leaderboard";
                    status = 404;
                    return null;
                }

                var kingdomsLeaderboard = new List<LeaderboardKingdomAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    var model = mapper.Map<LeaderboardKingdomAPIModel>(kingdom);
                    kingdomsLeaderboard.Add(model);
                }
                error = "ok";
                status = 200;
                return kingdomsLeaderboard.OrderByDescending(p => p.Total_points).ToList();

            }
            catch
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }

        public BattleTargetRespond Battle(BattleTargetRequest targetKingdom, long attackerId, string tokenUsername, out int status, out string error)
        {
            try
            {
                var attacker = FindPlayerInfoByKingdomId(attackerId);
                if(!DoesKingdomExist(targetKingdom.Target.KingdomId))
                {
                    error = "Target kingdom doesn't exist";
                    status = 404;
                    return null;
                }
                else if (targetKingdom.Target.KingdomId == attacker.Id)
                {
                    error = "You can't attack your own kingdom";
                    status = 405;
                    return null;
                }
                else if (!TroopQuantityCheck(targetKingdom, attacker))
                {
                    error = "You don't have enough troops";
                    status = 404;
                    return null;
                }
                else if (IsUserKingdomOwner(attackerId, tokenUsername))
                {
                    error = "ok";
                    status = 200;
                    return StartBattle(targetKingdom, attacker);
                }
                else
                {
                    error = "This kingdom does not belong to authenticated player";
                    status = 401;
                    return null;
                }
            }
            catch
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }

        public BattleResultRespond BattleInfo(long battleId, long kingdomId, string tokenUsername, out int status, out string error)
        {
            try
            {
                if (!IsUserKingdomOwner(kingdomId, tokenUsername))
                {
                    error = "This kingdom does not belong to authenticated player";
                    status = 401;
                    return null;
                }
                else if (!IsKingdomInBattle(battleId, kingdomId))
                {
                    error = "Kingdom didn't fight in selected battle";
                    status = 401;
                    return null;
                }
                else if (!DoesBattleExist(battleId))
                {
                    error = "Battle doesn't exist";
                    status = 404;
                    return null;
                }
                else
                {
                    error = "ok";
                    status = 200;
                    return GetBattleInfo(battleId);
                }
            }
            catch
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }

        public bool IsKingdomInBattle(long battleId, long kingdomId)
        {
            try
            {
                return UnitOfWork.Battles.Any(x => x.Id == battleId && (x.AttackerId == kingdomId || x.DefenderId == kingdomId));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool DoesBattleExist(long battleId)
        {
            try
            {
                return UnitOfWork.Battles.Any(x => x.Id == battleId);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public BattleResultRespond GetBattleInfo(long battleId)
        {
            try
            {
                var result = new BattleResultRespond();
                var battle = UnitOfWork.Battles.FirstOrDefault(x => x.Id == battleId);
                var lostTroopsAttacker = UnitOfWork.TroopsLost
                                                        .Where(x => x.BattleId == battleId && x.IsAttacker).ToList();
                var lostTroopsDefender = UnitOfWork.TroopsLost
                                                        .Where(x => x.BattleId == battleId && !(x.IsAttacker)).ToList();
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

                result = mapper.Map<BattleResultRespond>(battle);
                result.Attacker = attackerInfo;
                result.Defender = defenderInfo;
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public bool DoesKingdomExist(long kingdomId)
        {
            try
            {
                return UnitOfWork.Kingdoms.Any(x => x.Id == kingdomId);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public BattleTargetRespond StartBattle(BattleTargetRequest targetKingdom, Kingdom attacker)
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
                    FinishedAt = CountTravelTime(targetKingdom.Target.KingdomId, attacker.Id)
                };
                this.UnitOfWork.Battles.Add(battle);
                this.UnitOfWork.CompleteAsync();

                var attackerTroops = GetTroopLevels(attacker.Troops); 
                var attackingTroops = new List<AttackerTroops>();
                foreach (var troop in targetKingdom.Troops)
                {
                    UnitOfWork.AttackerTroops.Add(new AttackerTroops()
                    {
                        Type = troop.Type,
                        Quantity = troop.Quantity,
                        Level = attackerTroops.FirstOrDefault(x => x.Type == troop.Type).Level,
                        BattleId = battle.Id
                    });
                    UnitOfWork.CompleteAsync();
                    
                    //change all attacking troops status to attack that are in town currently
                    for (int i = 0; i < troop.Quantity; i++)
                    {
                        var troopStatus = attacker.Troops
                                                .FirstOrDefault(x => x.TroopType.Type == troop.Type && x.Status == "town");
                        troopStatus.Status = "attack";
                        UnitOfWork.Troops.UpdateState(troopStatus);
                        UnitOfWork.CompleteAsync();
                    }
                }
                return new BattleTargetRespond() 
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

        public Kingdom FindPlayerInfoByKingdomId(long kingdomId)
        {
            return UnitOfWork.Kingdoms.FindPlayerInfoByKingdomId(kingdomId);
        }

        public long CountTravelTime(long kingdomIdDef, long kingdomIdAtt)
        {
            try
            {
                var locationDef = UnitOfWork.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdDef)
                    .FirstOrDefault().Location;
                var locationAtt = UnitOfWork.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdAtt)
                    .FirstOrDefault().Location;

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
