using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class TroopsLost
    {
        public long Id { get; set; }
        public bool IsAttacker { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }

        public long BattleId { get; set; }
        public Battle Battle { get; set; }
    }
}
