using AutoMapper;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class KingdomAPIResponse
    {
        private readonly IMapper _mapper;

        public List<KingdomAPIModel> Kingdoms { get; set; }

        public KingdomAPIResponse(IMapper mapper, List<Kingdom> kingdoms)
        {
            _mapper = mapper;
            Kingdoms = KingdomListMapping(kingdoms, mapper);
        }

        private List<KingdomAPIModel> KingdomListMapping(List<Kingdom> kingdoms, IMapper mapper)
        {
            var kingdomAPIModels = new List<KingdomAPIModel>();

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
                var locationAPIModel = _mapper.Map<LocationAPIModel>(kingdom.Location);
                kingdomAPIModel.LocationAPIModel = locationAPIModel;
                kingdomAPIModels.Add(kingdomAPIModel);
            }

            return kingdomAPIModels;
        }
    }

}
