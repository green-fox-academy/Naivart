using AutoMapper;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class KingdomAPIResponse
    {
        public List<KingdomAPIModel> Kingdoms { get; set; }
        
    }
}

