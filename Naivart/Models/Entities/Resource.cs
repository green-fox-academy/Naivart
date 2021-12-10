using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Resource
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
    }
}
