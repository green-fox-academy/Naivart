using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class BuildingType
    {
        [Key]
        public long Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int GoldCost { get; set; }
        public int RequiredTownhallLevel { get; set; }
        public List<Building> Buildings { get; set; }
    }
}
