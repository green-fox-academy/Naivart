using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Battle
    {
        public long Id { get; set; }
        public long AttackerId { get; set; }
        public long DefenderId { get; set; }
        public string BattleType { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public int GoldStolen { get; set; }
        public int FoodStolen { get; set; }

        public List<AttackerTroops> AttackingTroops { get; set; }
        public List<TroopsLost> DeadTroops { get; set; }
    }
}
