using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Troops
{
    public class UpgradeTroopsResponse
    {
        public string Status { get; set; }

        public UpgradeTroopsResponse(string status)
        {
            Status = status;
        }
    }
}
