using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class PlayerWithKingdom
    {

        public string ruler { get; set; }
        public long kingdomId { get; set; }
        public string kingdomName { get; set; }
    }
}
