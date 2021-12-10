using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class BuildingApiResponse
    {
        public long kingdomId { get; set; }
        public string kingdomName { get; set; }
        public string ruler { get; set; }
        public int population { get; set; }
        public Location location { get; set; }
        public List<Building> buildings { get; set; }
    }
}
