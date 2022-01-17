using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class KingdomDetails
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<BuildingAPIModel> Buildings { get; set; }
        public List<ResourceAPIModel> Resources { get; set; }
        public List<TroopInfo> Troops { get; set; }

        public KingdomDetails(KingdomAPIModel kingdom, List<BuildingAPIModel> buildings, List<ResourceAPIModel> resources,  
            List<TroopInfo> troops)
        {
            Kingdom = kingdom;
            Buildings = buildings;
            Resources = resources;
            Troops = troops;
        }
    }
}
