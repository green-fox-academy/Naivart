using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class AllPlayerDetails
    {
        public KingdomInfo kingdom { get; set; }
        public List<ResourcesInfo> resources { get; set; }
        public List<BuildingsInfo> buildings { get; set; }
        public List<TroopsInfo> troops { get; set; }
    }
}
