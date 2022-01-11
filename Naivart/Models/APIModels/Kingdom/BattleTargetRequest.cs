using Naivart.Models.APIModels.Troops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class BattleTargetRequest
    {
        public TargetKingdomAPIModel Target { get; set; }
        public string BattleType { get; set; }
        public List<TroopsForFight> Troops { get; set; }
    }
}
