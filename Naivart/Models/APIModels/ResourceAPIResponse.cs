using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class ResourceAPIResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<ResourceAPIModel> Resources { get; set; }
    }
}

