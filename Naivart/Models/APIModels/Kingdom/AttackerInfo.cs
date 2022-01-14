using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class AttackerInfo
    {
        public ResourceStolen ResourceStolen { get; set; }
        public List<LostTroopsAPI> TroopsLost { get; set; }

        public AttackerInfo(ResourceStolen resourceStolen, List<LostTroopsAPI> troopsLost)
        {
            ResourceStolen = resourceStolen;
            TroopsLost = troopsLost;
        }
    }
}
