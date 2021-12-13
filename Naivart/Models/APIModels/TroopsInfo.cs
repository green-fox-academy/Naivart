using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class TroopsInfo
    {
        public long id { get; set; }
        public int level { get; set; }
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public DateTime? started_at { get; set; }
        public DateTime? finished_at { get; set; }
    }
}
