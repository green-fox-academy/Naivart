using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class TimeService
    {
        private IUnitOfWork UnitOfWork { get; set; }

        public TimeService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public long GetUnixTimeNow()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public async Task UpdateAllAsync(long kingdomId)    //maybe delete this later
        {
            await UpdateResourcesAsync(kingdomId);
            await UpdateBattleAsync(kingdomId);
            await UpdateTroopsAsync(kingdomId);
        }
        public async Task UpdateAllAsync(string username)       
        {
            //TODO: Update creating building/troops, upgrading buildings/troops
            //TODO: then make it more clean, to not run something that is not important
            var player = await UnitOfWork.Players.FindPlayerIncudeKingdomsByUsernameAsync(username);
            await UpdateResourcesAsync(player.KingdomId);
            await UpdateBattleAsync(player.KingdomId);
            await UpdateTroopsAsync(player.KingdomId);
        }

        public async Task UpdateTroopsAsync(long kingdomId)
        {
            var troops = await UnitOfWork.Troops.GetRecruitingTroops(kingdomId);
            foreach (var troop in troops)
            {
                if (troop.FinishedAt <= GetUnixTimeNow())
                {
                    troop.Status = "town";
                    await UnitOfWork.CompleteAsync();
                }
            }
        }

        public async Task UpdateResourcesAsync(long kingdomId)
        {
            var resources = await UnitOfWork.Resources.GetResourcesFromIdAsync(kingdomId);
            foreach (var resource in resources)
            {
                resource.Amount += CalculateAmount(resource.UpdatedAt, resource.Generation, out int extra);
                resource.UpdatedAt = GetUnixTimeNow() - extra;
                UnitOfWork.Resources.UpdateState(resource);
                await UnitOfWork.CompleteAsync();
            }
        }

        public int CalculateAmount(long lastUpdate, int generation, out int extra)
        {
            long secondsSinceLastCheck = GetUnixTimeNow() - lastUpdate;
            extra = Convert.ToInt32(((secondsSinceLastCheck % 600) * generation));
            return Convert.ToInt32(((secondsSinceLastCheck / 600) * generation));
        }

        public async Task UpdateBattleAsync(long kingdomId)
        {
            //check if kingdom is in or have any battles
            if (await UnitOfWork.Battles.IsKingdomInBattleAsync(kingdomId))
            {
                var battles = await UnitOfWork.Battles.BattlesAsync(kingdomId);
                foreach (var battle in battles)
                {
                    int totalDamage = 0;
                    int totalDefense = 0;
                    var defender = await UnitOfWork.Battles.DefenderAsync(battle.DefenderId);
                    var attacker = await UnitOfWork.Battles.AttackerAsync(battle.AttackerId);

                    //If fight didn't start yet and it's already time for it
                    if (battle.Result is null && battle.FinishedAt <= GetUnixTimeNow() && battle.Status != "done") 
                    {

                        foreach (var troops in battle.AttackingTroops)
                        { 
                            totalDamage += await UnitOfWork.TroopTypes.TotalDamageAsync(troops); //total damage is calculated based on attack * quantity * 6
                        }

                        //total defense is troops HP + defense
                        totalDefense = defender.Troops.Where(x => x.Status == "town")
                                                      .Sum(x => x.TroopType.Defense) + defender.Troops
                                                      .Sum(x => x.TroopType.Hp);
                    
                        if (totalDamage >= totalDefense) //if attacker won then true
                        {
                            await UpdateResourcesAsync(battle.DefenderId);     //just to make sure resources are up to date before stealing
                            int goldStolen;
                            int foodStolen;
                            int difference = totalDamage - totalDefense;
                            
                            if (difference == 0)
                            {
                                difference = 1;
                            }

                            int impactDamage = (difference) * 10;   
                        
                            //check how many golds and food player have if impact damage is greater than amount then steal all
                            if (impactDamage >= defender.Resources.FirstOrDefault(x => x.Type == "gold").Amount)    
                            {
                                goldStolen = defender.Resources.FirstOrDefault(x => x.Type == "gold").Amount;     
                            }
                            else
                            {
                                goldStolen = impactDamage;
                            }
                            if (impactDamage >= defender.Resources.FirstOrDefault(x => x.Type == "food").Amount)
                            {
                                foodStolen = defender.Resources.FirstOrDefault(x => x.Type == "food").Amount;
                            }
                            else
                            {
                                foodStolen = impactDamage;
                            }

                            //update battle and time when troops get back to town
                            battle.GoldStolen = goldStolen;
                            battle.FoodStolen = foodStolen;
                            battle.Result = "attacker won";
                            long travelTime = battle.FinishedAt - battle.StartedAt - (GetUnixTimeNow() - battle.FinishedAt);
                            battle.FinishedAt += travelTime;
                            battle.Status = "on way back";
                        
                            UnitOfWork.Battles.UpdateState(battle);
                            await UnitOfWork.CompleteAsync();

                            //update resources for defender
                            defender.Resources.FirstOrDefault(x => x.Type == "gold").Amount -= goldStolen;
                            defender.Resources.FirstOrDefault(x => x.Type == "food").Amount -= foodStolen;
                        
                            UnitOfWork.Kingdoms.UpdateState(defender);
                            await UnitOfWork.CompleteAsync();

                            //saving dead troops list of DEFENDER and remove from kingdom
                            await SaveAndRemoveTroopsLostDefenderAsync(defender.Troops, battle.Id);

                            //saving dead troops list of ATTACKER and remove from kingdom
                            double points = difference;
                            double ap = totalDamage;
                            double dif = ap - difference;
                            double attResult;
                            double quantityResult;

                            var attackerTroops = battle.AttackingTroops;
                            foreach (var troop in attackerTroops)
                            {
                                attResult = dif / ap;
                                quantityResult = troop.Quantity * attResult;
                                if (Math.Round(quantityResult) != troop.Quantity || Math.Round(quantityResult) != 0)
                                {
                                    var lostTroop = new TroopsLost()
                                    {
                                        IsAttacker = true,
                                        BattleId = battle.Id,
                                        Type = troop.Type,
                                        Quantity = Convert.ToInt32(Math.Round(quantityResult))
                                    };
                                    UnitOfWork.TroopsLost.AddAsync(lostTroop);
                                    await UnitOfWork.CompleteAsync();
                                }
                                else if (Math.Round(quantityResult) != 0)
                                {
                                    var lostTroop = new TroopsLost()
                                    {
                                        IsAttacker = true,
                                        BattleId = battle.Id,
                                        Type = troop.Type,
                                        Quantity = (troop.Quantity - 1) == 0 ? 1 : (troop.Quantity - 1)
                                    };
                                    UnitOfWork.TroopsLost.AddAsync(lostTroop);
                                    await UnitOfWork.CompleteAsync();
                                }
                            }
                        }
                        else    //defender won
                        {
                            int difference = totalDefense - totalDamage;

                            //update battle and time when troops get back to town
                            battle.Result = "attacker lost";
                            long travelTime = battle.FinishedAt - battle.StartedAt - (GetUnixTimeNow() - battle.FinishedAt);
                            battle.FinishedAt += travelTime;
                            battle.Status = "done";

                            UnitOfWork.Battles.UpdateState(battle);
                            await UnitOfWork.CompleteAsync();

                            //saving dead troops list of ATTACKER and remove from kingdom
                            await SaveAndRemoveTroopsLostAttackerAsync(battle.AttackingTroops, battle.Id, attacker);

                            //saving dead troops list of DEFENDER and remove from kingdom
                            double points = difference;
                            double dp = totalDefense;
                            double dif = dp - difference;
                            double attResult;
                            double quantityResult;

                            var defenderTroops = GetTroopQuantity(defender.Troops);
                            var troopsForRemove = new List<Troop>();

                            foreach (var troop in defenderTroops)
                            {
                                attResult = dif / dp;
                                quantityResult = troop.Quantity * attResult;

                                if (Math.Round(quantityResult) != troop.Quantity || Math.Round(quantityResult) != 0) 
                                {
                                    var lostTroop = new TroopsLost()
                                    {
                                        IsAttacker = false,
                                        BattleId = battle.Id,
                                        Type = troop.Type,
                                        Quantity = Convert.ToInt32(Math.Round(quantityResult))
                                    };
                                    UnitOfWork.TroopsLost.AddAsync(lostTroop);
                                    await UnitOfWork.CompleteAsync();
                                }
                                else if(Math.Round(quantityResult) != 0)
                                {
                                    var lostTroop = new TroopsLost()
                                    {
                                        IsAttacker = false,
                                        BattleId = battle.Id,
                                        Type = troop.Type,
                                        Quantity = (troop.Quantity - 1) == 0 ? 1 : (troop.Quantity - 1)
                                    };
                                    UnitOfWork.TroopsLost.AddAsync(lostTroop);
                                    await UnitOfWork.CompleteAsync();
                                }
                            }
                            var lostTroops = await UnitOfWork.TroopsLost.LostTroopsDefenderAsync(battle.Id);
                            foreach (var troop in lostTroops)
                            {
                                troopsForRemove = defender.Troops
                                    .Where(x => x.TroopType.Type == troop.Type && x.Status == "town")
                                    .Take(troop.Quantity).ToList();
                                UnitOfWork.Troops.RemoveRange(troopsForRemove);
                                await UnitOfWork.CompleteAsync();
                            }
                            
                        }
                    }
                    //if true, then troops are comming back to town after battle with stolen resources
                    if (battle.Result is not null && battle.FinishedAt <= GetUnixTimeNow() && battle.Status != "done")     
                    {
                        battle.Status = "done";
                        UnitOfWork.Battles.UpdateState(battle);
                        await UnitOfWork.CompleteAsync();

                        var attackTroops = battle.DeadTroops
                                                        .Where(x => x.IsAttacker && x.BattleId == battle.Id).ToList();
                        var troopsForRemove = new List<Troop>();
                        var troopsForUpdate = new List<Troop>();

                        foreach (var troop in attackTroops) 
                        {
                            troopsForRemove = attacker.Troops
                                .Where(x => x.TroopType.Type == troop.Type && x.Status == "attack").Take(troop.Quantity).ToList();
                            
                            UnitOfWork.Troops.RemoveRange(troopsForRemove);
                            await UnitOfWork.CompleteAsync();
                        }

                        attacker.Resources.FirstOrDefault(x => x.Type == "gold").Amount += battle.GoldStolen;
                        attacker.Resources.FirstOrDefault(x => x.Type == "food").Amount += battle.FoodStolen;

                        UnitOfWork.Kingdoms.UpdateState(attacker);
                        await UnitOfWork.CompleteAsync();

                        foreach (var troop in attackTroops)
                        {
                            for (int i = 0; i < battle.AttackingTroops
                                .FirstOrDefault(x => x.Type == troop.Type).Quantity - troop.Quantity; i++)
                            {
                                var attTroop = attacker.Troops
                                    .FirstOrDefault(x => x.TroopType.Type == troop.Type && x.Status == "attack");
                                attTroop.Status = "town";
                                troopsForUpdate.Add(attTroop);
                            }
                        }
                        await UnitOfWork.Battles.UpdateTroopsAsync(troopsForUpdate);
                    }
                }
            }
        }
        public List<TroopBattleInfo> GetTroopQuantity(List<Troop> input)
        {
            var troopQuantity = new List<TroopBattleInfo>();
            foreach (var troop in input)
            {
                if (troopQuantity.Any(x => x.Type == troop.TroopType.Type && x.Level == troop.TroopType.Level))
                {
                    troopQuantity.FirstOrDefault(x => x.Type == troop.TroopType.Type && x.Level == troop.TroopType.Level)
                                    .Quantity++;
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

        public async Task SaveAndRemoveTroopsLostAttackerAsync(List<AttackerTroops> attackerTroops, long battleId, Kingdom attacker)
        {
            var deadTroops = new List<TroopsLost>();
            var troopsForRemove = new List<Troop>();
            foreach (var troops in attackerTroops)
            {
                deadTroops.Add(new TroopsLost() 
                {
                    BattleId = battleId,
                    IsAttacker = true,
                    Type = troops.Type,
                    Quantity = troops.Quantity
                });

                troopsForRemove = attacker.Troops.Where(x => x.TroopType.Type == troops.Type && x.Status == "attack")
                    .Take(troops.Quantity).ToList();
            }

            foreach (var troops in deadTroops)
            {
                UnitOfWork.TroopsLost.AddAsync(troops);
                await UnitOfWork.CompleteAsync();
            }
            
            UnitOfWork.Troops.RemoveRange(troopsForRemove);
            await UnitOfWork.CompleteAsync();
        }
        public async Task SaveAndRemoveTroopsLostDefenderAsync(List<Troop> input, long battleId)
        {
            var deadTroops = new List<TroopsLost>();
            foreach (var troop in input)
            {
                if (deadTroops.Any(x => x.Type == troop.TroopType.Type))
                {
                    deadTroops.FirstOrDefault(x => x.Type == troop.TroopType.Type).Quantity++;
                }
                else
                {
                    deadTroops.Add(new TroopsLost()
                    {
                        BattleId = battleId,
                        IsAttacker = false,
                        Type = troop.TroopType.Type,
                        Quantity = 1
                    });
                }
            }

            foreach (var troops in deadTroops)
            {
                UnitOfWork.TroopsLost.AddAsync(troops);
                await UnitOfWork.CompleteAsync();
            }

            UnitOfWork.Troops.RemoveRange(input);
            await UnitOfWork.CompleteAsync();
        }
    }
}
