using System.Collections.Generic;

namespace Naivart.Models.APIModels.Troops
{
    public class TroopsResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<TroopAPIModel> Troops { get; set; }

        public TroopsResponse(KingdomAPIModel kingdom, List<TroopAPIModel> troops)
        {
            Kingdom = kingdom;
            Troops = troops;
        }
    }
}
