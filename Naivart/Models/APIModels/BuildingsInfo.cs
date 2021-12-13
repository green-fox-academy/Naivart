using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class BuildingsInfo
    {
        public long id { get; set; }
        public string type { get; set; }
        public int level { get; set; }
        public long started_at { get; set; }
        public long finished_at { get; set; }
    }
}
