using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.BuildingTypes
{
    public class Academy : BuildingModel
    {
        public Academy()
        {
            GoldCost = 300;
            RequestTownhallLevel = 4;
            Type = "academy";
            Level = 1;
            Hp = 200;
        }
    }
}
