using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class TroopType
    {
        [Key]
        public long Id { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int GoldCost { get; set; }
        public int DailyFoodCost { get; set; }
        public List<Troop> Troops { get; set; }
    }
}
