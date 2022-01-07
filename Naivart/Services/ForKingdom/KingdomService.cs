using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
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
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public LoginService LoginService { get; set; }
        public BuildingService BuildingService { get; set; }
        public TimeService TimeService { get; set; }
        public KingdomService(IMapper mapper, ApplicationDbContext dbContext,
                              AuthService authService, LoginService loginService, TimeService timeService)
        {
            this.mapper = mapper;
            DbContext = dbContext;
            AuthService = authService;
            LoginService = loginService;
            TimeService = timeService;
        }

        public List<Kingdom> GetAll()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return kingdoms = DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .Include(k => k.Buildings)
                    .Include(k => k.Resources)
                    .Include(k => k.Troops)
                    .ThenInclude(k => k.TroopType)
                    .ToList();
            }
            catch
            {
                return kingdoms;
            }
        }

        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                return kingdom = GetAll().FirstOrDefault(k => k.Id == id);
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
            Kingdom kingdom = GetById(kingdomId);
            kingdom.Name = newKingdomName;
            DbContext.Update(kingdom);
            DbContext.SaveChanges();
            return kingdom;
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
                return DbContext.Locations.Any(x => x.CoordinateX == input.CoordinateX && x.CoordinateY == input.CoordinateY);
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
                return DbContext.Kingdoms.Any(x => x.Id == input.KingdomId && x.LocationId == null);
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
                DbContext.Locations.Add(model);
                DbContext.SaveChanges();
                long locationId = DbContext.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX && x.CoordinateY == model.CoordinateY).Id;
                DbContext.Kingdoms.FirstOrDefault(x => x.Id == input.KingdomId).LocationId = locationId;
                DbContext.SaveChanges();
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
                return DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId)
                                        .Username == username;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool IsEnoughGoldFor(int goldAmount, string operation)
        {
            var operations = new Dictionary<string, int>()
            {
                ["upgrade building"] = 10
            };
            return goldAmount >= operations[operation];
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
                var allKingdoms = GetAll();
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

        public void Battle(BattleTargetRequest targetKingdom, long attackerId, string tokenUsername, out int status, out string error)
        {
            try
            {
                if (IsUserKingdomOwner(attackerId, tokenUsername))
                {
                    error = "ok";
                    status = 200;
                    StartBattle(targetKingdom, FindPlayerInfoByKingdomId(attackerId));
                }
                else
                {
                    error = "This kingdom does not belong to authenticated player";
                    status = 401;
                }
            }
            catch
            {
                error = "Data could not be read";
                status = 500;
            }
        }

        public void StartBattle(BattleTargetRequest targetKingdom, Kingdom attacker)
        {
            if (!TroopQuantityCheck(targetKingdom, attacker) || targetKingdom.Target.KingdomId == attacker.Id)
            {
                //Bad quantity or you attack yourself
            }
            else
            {
                //TODO
                //take Troops to battle (true/false?) send them to new battle DB, create new Battle Id, save battle result into DB
                //send back troops that survived or kill that did not
                var battle = new Battle()
                {
                    AttackerId = attacker.Id,
                    DefenderId = targetKingdom.Target.KingdomId,
                    BattleType = targetKingdom.BattleType,
                    Status = "on way attack",
                    StartedAt = TimeService.GetUnixTimeNow(),
                    FinishedAt = CountTravelTime(targetKingdom.Target.KingdomId, attacker.Id)
                };
                this.DbContext.Battles.Add(battle);
                this.DbContext.SaveChanges();

                var attackerTroops = GetTroopLevels(attacker.Troops); 
                var attackingTroops = new List<AttackerTroops>();
                foreach (var troop in targetKingdom.Troops)
                {
                    DbContext.AttackerTroops.Add(new AttackerTroops()
                    {
                        Type = troop.Type,
                        Quantity = troop.Quantity,
                        Level = attackerTroops.FirstOrDefault(x => x.Type == troop.Type).Level,
                        BattleId = battle.Id
                    });
                    DbContext.SaveChanges();
                    
                    //change all attacking troops status to attack that are in town currently
                    for (int i = 0; i < troop.Quantity; i++)
                    {
                        var troopStatus = attacker.Troops
                                              .FirstOrDefault(x => x.TroopType.Type == troop.Type && x.Status == "town");
                        troopStatus.Status = "attack";
                        DbContext.Update(troopStatus);
                        DbContext.SaveChanges();
                    }
                }
            }
        }

        public bool TroopQuantityCheck(BattleTargetRequest targetKingdom, Kingdom attacker)
        {
            var troopQuantity = GetTroopQuantity(attacker.Troops.Where(x => x.Status == "town").ToList());

            return targetKingdom.Troops.All(x => troopQuantity.Select(x => x.Type).Contains(x.Type)) &&  
                    //checks if you have all types of troops needed
                   targetKingdom.Troops.All(x => troopQuantity.Any(y => y.Type == x.Type && y.Quantity >= x.Quantity)); 
                    //checks if all types have correct amount of troops
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
            return DbContext.Kingdoms.Where(x => x.Id == kingdomId).Include(x => x.Troops).ThenInclude(x => x.TroopType).FirstOrDefault();
        }

        public long CountTravelTime(long kingdomIdDef, long kingdomIdAtt)
        {
            var locationDef = DbContext.Kingdoms.Where(x => x.Id == kingdomIdDef).Include(x => x.Location)
                .FirstOrDefault().Location;
            var locationAtt = DbContext.Kingdoms.Where(x => x.Id == kingdomIdAtt).Include(x => x.Location)
                .FirstOrDefault().Location;

            //analytic geometry - difference between two points
            double resultA = (locationAtt.CoordinateX - locationDef.CoordinateX) * (locationAtt.CoordinateX - locationDef.CoordinateX)
                            + (locationAtt.CoordinateY - locationDef.CoordinateY) * (locationAtt.CoordinateY - locationDef.CoordinateY);
            double resultB = Math.Sqrt(resultA);
            //travel speed 1 point = 10min => longest distance takes about 24hours
            double resultC = Math.Round((resultB * 600) + TimeService.GetUnixTimeNow());
            
            return Convert.ToInt64(resultC);
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
