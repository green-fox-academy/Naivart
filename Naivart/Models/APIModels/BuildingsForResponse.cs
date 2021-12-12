using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class BuildingsForResponse
    {
        public long id { get; set; }
        public string type { get; set; }
        public int level { get; set; }
        public long startedAt { get; set; }
        public long finishedAt { get; set; }
    }
}
