using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class CreateTroopAPIRequest
    {
        public string Type { get; set; }
        public int Quantity { get; set; }
    }
}
