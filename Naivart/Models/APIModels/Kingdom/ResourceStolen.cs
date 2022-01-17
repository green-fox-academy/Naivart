using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Kingdom
{
    public class ResourceStolen
    {
        public int Food { get; set; }
        public int Gold { get; set; }

        public ResourceStolen(int food, int gold)
        {
            Food = food;
            Gold = gold;
        }
    }
}
