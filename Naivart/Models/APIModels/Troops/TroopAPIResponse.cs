using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Troops
{
    public class TroopAPIResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<TroopAPIModel> Troops { get; set; }
    }
}
