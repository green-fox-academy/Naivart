using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class AddBuildingForResponse
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
    }
}
