using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class KingdomLocationInput
    {
        public int coordinateY { get; set; }
        public int coordinateX { get; set; }
        public long kingdomId { get; set; }
    }
}
