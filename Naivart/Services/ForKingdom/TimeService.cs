using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using System;
using System.Linq;

namespace Naivart.Services
{
    public class TimeService
    {
        private ApplicationDbContext DbContext { get; }
        public TimeService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public long GetUnixTimeNow()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        
        public void UpdateResources(long kingdomId)
        {
            var resources = DbContext.Resources.Where(x => x.KingdomId == kingdomId).ToList();
            foreach (var resource in resources)
            {
                resource.Amount += CalculateAmount(resource.UpdatedAt, resource.Generation, out int extra);
                resource.UpdatedAt = GetUnixTimeNow() - extra;
                DbContext.Resources.Update(resource);
                DbContext.SaveChanges();
            }
        }

        public int CalculateAmount(long lastUpdate, int generation, out int extra)
        {
            long secondsSinceLastCheck = GetUnixTimeNow() - lastUpdate;
            extra = Convert.ToInt32(((secondsSinceLastCheck % 600) * generation));
            return Convert.ToInt32(((secondsSinceLastCheck / 600) * generation));
        }

        public void UpdateBattle(long kingdomId)
        {
            var battles = DbContext.Battles.Where(x => x.AttackerId == kingdomId || x.DefenderId == kingdomId)
                                    .Include(x => x.AttackingTroops)?.Include(x => x.DeadTroops).ToList();
            foreach (var battle in battles)
            {
                int totalDamage = 0;
                int totalDefense = 0;
                if (battle.Result is null && battle.FinishedAt <= GetUnixTimeNow()) //If fight didn't start yet and it's already time for it
                {
                    foreach (var troops in battle.AttackingTroops)
                    {
                        totalDamage += DbContext.TroopTypes.FirstOrDefault(x => x.Type == troops.Type && x.Level == troops.Level).Attack 
                                        * 6 * troops.Quantity;      //total damage is calculated based on attack * quantity * 6
                    }
                    var defender = DbContext.Kingdoms.Where(x => x.Id == battle.DefenderId).Include(x => x.Troops)
                                    .ThenInclude(x => x.TroopType).Include(x => x.Resources).FirstOrDefault();

                    totalDefense = defender.Troops.Sum(x => x.TroopType.Defense) + defender.Troops.Sum(x => x.TroopType.Hp);
                    
                    if (totalDamage >= totalDefense) //if attacker won then true
                    {
                        int goldStolen;
                        int foodStolen;
                        int impactDamage = (totalDamage - totalDefense) * 10;
                        
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
                        
                        DbContext.Battles.Update(battle);
                        DbContext.SaveChanges();

                        //TODO dead troops list and decrease number of resources for defender
                    }
                    else    //defender won
                    {
                        //TODO dead troops list, resources stolen stays on 0
                    }
                }
                if (battle.Result is not null && battle.FinishedAt <= GetUnixTimeNow())
                {
                    //TODO add stolen resources, delete troops based on troopsLost, battle status (on way attack, on way home, done)
                }
            }
        }
    }
}
