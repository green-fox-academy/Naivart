using System.Collections.Generic;

namespace Naivart.Models.APIModels.Troops
{
    public class TroopAPIResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<TroopAPIModel> Troops { get; set; }
    }
}
