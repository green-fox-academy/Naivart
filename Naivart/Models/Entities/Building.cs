using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Building
    {
        [Key]
        public long Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}
