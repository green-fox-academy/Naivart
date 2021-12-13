using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class AllPlayerDetails
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<ResourceAPIModel> Resources { get; set; }
        public List<BuildingsInfo> Buildings { get; set; }
        public List<TroopsInfo> Troops { get; set; }
    }
}
