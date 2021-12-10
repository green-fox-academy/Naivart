using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Troop
    {
        [Key]
        public long Id { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public DateTime? Started_at { get; set; }
        public DateTime? Finished_at { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}
