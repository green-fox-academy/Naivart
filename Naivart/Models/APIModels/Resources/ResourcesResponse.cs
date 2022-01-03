using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class ResourcesResponse
    {
        public KingdomAPIModel Kingdom { get; set; }
        public List<ResourceAPIModel> Resources { get; set; }
    }
}

