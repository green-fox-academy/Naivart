using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Buildings
{
    public class BuildingModel
    {
        public string Type { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public long KingdomId { get; set; }
        public long BuildingTypeId { get; set; }

        public BuildingModel(BuildingModel model, long kingdomId, long buildingTypeId)
        {
            KingdomId = kingdomId;
            BuildingTypeId = buildingTypeId;
        }
    }
}
