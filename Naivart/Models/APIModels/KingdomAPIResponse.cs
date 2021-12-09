using AutoMapper;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Models.APIModels
{
    public class KingdomAPIResponse
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public List<KingdomAPIModel> Kingdoms { get; set; }

        public KingdomAPIResponse()
        {

        }
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
                kingdomAPIModel.Location = locationAPIModel;
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
        }
    }

}
