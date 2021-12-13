using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class KingdomInfo
    {
        public long kingdomId { get; set; }
        public string kingdomName { get; set; }
        public string ruler { get; set; }
        public int population { get; set; }
        public LocationAPIModel location { get; set; }
    }
}
