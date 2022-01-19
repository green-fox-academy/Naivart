using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Interfaces.ServiceInterfaces;
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
    public class KingdomService : IKingdomService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public AuthService AuthService { get; set; }
        public LoginService LoginService { get; set; }
        public TimeService TimeService { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }
        public KingdomService(IMapper mapper, AuthService authService, LoginService loginService, TimeService timeService,
                              IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            AuthService = authService;
            LoginService = loginService;
            TimeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Kingdom>> GetAllAsync()
        {
            return await _unitOfWork.Kingdoms.GetAllKingdomsAsync();
        }

        public async Task<Kingdom> GetByIdAsync(long id)
        {
            try
            {
                return (await _unitOfWork.Kingdoms.GetAllKingdomsAsync()).FirstOrDefault(k => k.Id == id);
            }
            catch
            {
                return new Kingdom();
            }
        }

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
            kingdomAPIModel.Location = _mapper.Map<LocationAPIModel>(kingdom.Location);
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
                kingdomAPIModels.Add(KingdomMapping(kingdom));
            }
            return kingdomAPIModels;
        }

        public async Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName)
        {
            return await _unitOfWork.Kingdoms.RenameKingdomAsync(kingdomId, newKingdomName);
        }

        public async Task<(int status, string message)> RegisterKingdomAsync(KingdomLocationInput input, string usernameToken)
        {
            try
            {
                if (!IsCorrectLocationRange(input))
                {
                    return (400, "One or both coordinates are out of valid range (0-99).");
                }
                else if (await _unitOfWork.Locations.IsLocationTakenAsync(input))
                {
                    return (400, "Given coordinates are already taken!");
                }
                else if (await _unitOfWork.Kingdoms.HasAlreadyLocationAsync(input))
                {
                    if (await IsUserKingdomOwnerAsync(input.KingdomId, usernameToken))
                    {
                        await ConnectLocationAsync(input);
                        return (200, "OK");
                    }
                    return (401, "Wrong user authentication!");
                }
                return (409, "Your kingdom already have location!");
            }
            catch
            {
                return (500, "Data could not be read");
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.CoordinateX >= 0 && input.CoordinateX <= 99 && input.CoordinateY >= 0 && input.CoordinateY <= 99;
        }

        public async Task ConnectLocationAsync(KingdomLocationInput input)
        {
            try
            {
                var location = new Location(input.CoordinateX, input.CoordinateY);
                _unitOfWork.Locations.AddAsync(location);
                await _unitOfWork.CompleteAsync();

                long locationId = await _unitOfWork.Locations.GetLocationIdFromCoordinatesAsync(location);
                await _unitOfWork.Kingdoms.ChangeLocationIdForKingdomAsync(input, locationId);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<(KingdomDetails model, int status, string message)> GetKingdomInfoAsync(long kingdomId, string tokenUsername)
        {
            try
            {
                if (await IsUserKingdomOwnerAsync(kingdomId, tokenUsername))
                {
                    return (await GetAllInfoAboutKingdomAsync(kingdomId), 200, "OK"); //this will use automapper to create an object
                }
                else
                {
                    return (null, 401, "This kingdom does not belong to authenticated player");
                }
            }
            catch
            {
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
                    buildings.Add(_mapper.Map<BuildingAPIModel>(building));
                }

                foreach (var resource in kingdom.Resources)   //list of resources
                {
                    resources.Add(_mapper.Map<ResourceAPIModel>(resource));
                }

                foreach (var troop in kingdom.Troops)   //list of troops
                {
                    troops.Add(_mapper.Map<TroopInfo>(troop));
                }

                return new KingdomDetails(_mapper.Map<KingdomAPIModel>(kingdom), buildings, resources, troops);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username)
        {
            return await _unitOfWork.Players.IsUserKingdomOwnerAsync(kingdomId, username);
        }

        public async Task<int> GetGoldAmountAsync(long kingdomId)
        {
            try
            {
                return (await GetByIdAsync(kingdomId)).Resources.FirstOrDefault(x => x.Type == "gold").Amount;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public async Task<(List<LeaderboardKingdomAPIModel> model, int status, string message)> GetKingdomsLeaderboardAsync()
        {
            try
            {
                var allKingdoms = await _unitOfWork.Kingdoms.GetAllKingdomsAsync();
                if (allKingdoms.Count == 0)
                {
                    return (null, 404, "There are no kingdoms in Leaderboard");
                }

                var kingdomsLeaderboard = new List<LeaderboardKingdomAPIModel>();
                foreach (var kingdom in allKingdoms)
                {
                    kingdomsLeaderboard.Add(_mapper.Map<LeaderboardKingdomAPIModel>(kingdom));
                }
                return (kingdomsLeaderboard.OrderByDescending(p => p.Total_points).ToList(), 200, "OK");

            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<(BattleTargetResponse model, int status, string message)> BattleAsync(
            BattleTargetRequest targetKingdom, long attackerId, string tokenUsername)
        {
            try
            {
                var attacker = await _unitOfWork.Kingdoms.FindPlayerInfoByKingdomIdAsync(attackerId);
                if (!(await _unitOfWork.Kingdoms.DoesKingdomExistAsync(targetKingdom.Target.KingdomId)) 
                    || attacker is null)
                {
                    return (null, 404, "Target kingdom doesn't exist");
                }
                else if (targetKingdom.Target.KingdomId == attacker.Id)
                {
                    return (null, 405, "You can't attack your own kingdom!");
                }
                else if (!TroopQuantityCheck(targetKingdom, attacker))
                {
                    return (null, 404, "You don't have enough troops!");
                }
                else if (await IsUserKingdomOwnerAsync(attackerId, tokenUsername))
                {
                    return (await StartBattleAsync(targetKingdom, attacker), 200, "OK");
                }
                else
                {
                    return (null, 401, "This kingdom does not belong to authenticated player!");
                }
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<(BattleResultResponse model, int status, string message)> BattleInfoAsync(
            long battleId, long kingdomId, string tokenUsername)
        {
            try
            {
                if (!await IsUserKingdomOwnerAsync(kingdomId, tokenUsername))
                {
                    return (null, 401, "This kingdom does not belong to authenticated player!");
                }
                else if (!await _unitOfWork.Battles.IsKingdomInBattleAsync(battleId, kingdomId))
                {
                    return (null, 401, "Kingdom didn't fight in selected battle!");
                }
                else if (!await _unitOfWork.Battles.DoesBattleExistAsync(battleId))
                {
                    return (null, 404, "Battle doesn't exist!");
                }
                else
                {
                    return (await GetBattleInfoAsync(battleId), 200, "OK");
                }
            }
            catch
            {
                return (null, 500, "Data could not be read");
            }
        }

        public async Task<BattleResultResponse> GetBattleInfoAsync(long battleId)
        {
            try
            {
                var battle = await _unitOfWork.Battles.GetBattleFromBattleIdAsync(battleId);

                var lostTroopsAttacker = await _unitOfWork.TroopsLost.LostTroopsAttackerAsync(battleId);
                var lostTroopsDefender = await _unitOfWork.TroopsLost.LostTroopsDefenderAsync(battleId);

                var stolenResources = new ResourceStolen(battle.FoodStolen, battle.GoldStolen);

                var modelAttackerTroopsLost = new List<LostTroopsAPI>();
                foreach (var item in lostTroopsAttacker)
                {
                    modelAttackerTroopsLost.Add(_mapper.Map<LostTroopsAPI>(item));
                }

                var modelDefenderTroopsLost = new List<LostTroopsAPI>();
                foreach (var item in lostTroopsDefender)
                {
                    modelDefenderTroopsLost.Add(_mapper.Map<LostTroopsAPI>(item));
                }

                var attackerInfo = new AttackerInfo(stolenResources, modelAttackerTroopsLost);
                var defenderInfo = new DefenderInfo(modelDefenderTroopsLost);

                var result = _mapper.Map<BattleResultResponse>(battle);
                result.Attacker = attackerInfo;
                result.Defender = defenderInfo;
                return result;
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
                var battle = new Battle(attacker.Id, targetKingdom.Target.KingdomId, targetKingdom.BattleType, "on way attack",
                    TimeService.GetUnixTimeNow(), await CountTravelTimeAsync(targetKingdom.Target.KingdomId, attacker.Id));
                _unitOfWork.Battles.AddAsync(battle);
                await _unitOfWork.CompleteAsync();

                var attackerTroops = GetTroopLevels(attacker.Troops);
                foreach (var troop in targetKingdom.Troops)
                {
                    _unitOfWork.AttackerTroops.AddAsync(new AttackerTroops(troop.Type, troop.Quantity, 
                        attackerTroops.FirstOrDefault(x => x.Type == troop.Type).Level, battle.Id));
                    await _unitOfWork.CompleteAsync();

                    //change all attacking troops status to attack that are in town currently
                    for (int i = 0; i < troop.Quantity; i++)
                    {
                        var troopStatus = attacker.Troops.FirstOrDefault(x => x.TroopType.Type == troop.Type &&
                                                                                        x.Status == "town");
                        troopStatus.Status = "attack";
                        _unitOfWork.Troops.UpdateState(troopStatus);
                        await _unitOfWork.CompleteAsync();
                    }
                }
                return new BattleTargetResponse(battle.Id, battle.FinishedAt);
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

        public List<TroopBattleInfo> GetTroopQuantity(List<Troop> troops)
        {
            var troopQuantity = new List<TroopBattleInfo>();
            foreach (var troop in troops.Select(t => t.TroopType))
            {
                if (troopQuantity.Any(x => x.Type == troop.Type && x.Level == troop.Level))
                {
                    troopQuantity.FirstOrDefault(x => x.Type == troop.Type && x.Level == troop.Level).Quantity++;
                }
                else
                {
                    troopQuantity.Add(new TroopBattleInfo(troop.Type, 1, troop.Level));
                }
            }
            return troopQuantity;
        }

        public async Task<long> CountTravelTimeAsync(long kingdomIdDef, long kingdomIdAtt)
        {
            try
            {
                var locationDef = await _unitOfWork.Locations.LocationDefAsync(kingdomIdDef);
                var locationAtt = await _unitOfWork.Locations.LocationDefAsync(kingdomIdAtt);

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

        public List<TroopBattleInfo> GetTroopLevels(List<Troop> troops)
        {
            var output = new List<TroopBattleInfo>();
            foreach (var troop in troops.Select(t => t.TroopType))
            {
                if (!output.Any(x => x.Type == troop.Type && x.Level == troop.Level))
                {
                    output.Add(new TroopBattleInfo(troop.Type, troop.Level));
                }
            }
            return output;
        }
    }
}
