using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class BuildingsInfo
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public long Started_at { get; set; }
        public long Finished_at { get; set; }
    }
}
