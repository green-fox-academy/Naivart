using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class ResourceAPIModel
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }
    }
}
