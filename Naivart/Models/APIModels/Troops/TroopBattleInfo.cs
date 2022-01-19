using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Troops
{
    public class TroopBattleInfo
    {
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int Level { get; set; }

        public TroopBattleInfo(string type, int quantity, int level)
        {
            Type = type;
            Quantity = quantity;
            Level = level;
        }

        public TroopBattleInfo(string type, int level)
        {
            Type = type;
            Level = level;
        }

        public TroopBattleInfo()
        {

        }
    }
}
