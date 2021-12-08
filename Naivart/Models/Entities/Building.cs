using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Building
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}
