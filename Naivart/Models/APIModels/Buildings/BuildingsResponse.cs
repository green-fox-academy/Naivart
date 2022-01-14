using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class BuildingsResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<BuildingAPIModel> Buildings { get; set; }

        public BuildingsResponse(KingdomAPIModel kingdom, List<BuildingAPIModel> buildings)
        {
            Kingdom = kingdom;
            Buildings = buildings;
        }
    }
}
