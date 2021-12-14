using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.TroopTypes
{
    public class Recruit : TroopModel
    {
        public Recruit()
        {
            GoldCost = 10;
            Type = "recruit";
            Level = 1;
            Hp = 10;
            Attack = 1;
            Defence = 2;
        }
    }
}
