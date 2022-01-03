using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class TargetKingdomAPIModel
    {
        public long KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public LocationAPIModel Location { get; set; }
    }
}
