using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class ResourcesInfo
    {
        public string type { get; set; }
        public int amount { get; set; }
        public int generation { get; set; }
        public long updatedAt { get; set; }
    }
}
