using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class BattleResultResponse
    {
        public long BattleId { get; set; }
        public long ResolutionTime { get; set; }
        public string BattleType { get; set; }
        public string Result { get; set; }
        public AttackerInfo Attacker { get; set; }
        public DefenderInfo Defender { get; set; }

        public BattleResultResponse(BattleResultResponse response, AttackerInfo attacker, DefenderInfo defender)
        {
            Attacker = attacker;
            Defender = defender;
        }
        public BattleResultResponse()
        {

        }
    }
}
