using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class DefenderInfo
    {
        public List<LostTroopsAPI> TroopsLost { get; set; }

        public DefenderInfo(List<LostTroopsAPI> troopsLost)
        {
            TroopsLost = troopsLost;
        }
    }
}
