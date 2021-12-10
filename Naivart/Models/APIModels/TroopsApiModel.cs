using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class TroopsApiModel
    {
        public Kingdom Kingdom { get; set; }
        public List<Troop> Troops { get; set; }
    }
}
