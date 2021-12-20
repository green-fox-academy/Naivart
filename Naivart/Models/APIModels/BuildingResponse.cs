using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class BuildingResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<BuildingsForResponse> Buildings { get; set; }
    }
}
