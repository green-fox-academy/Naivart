using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class KingdomsResponse
    {
        public List<KingdomAPIModel> Kingdoms { get; set; }

        public KingdomsResponse(List<KingdomAPIModel> kingdoms)
        {
            Kingdoms = kingdoms;
        }
    }
}

