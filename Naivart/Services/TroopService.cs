using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Services
{
    public class TroopService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public TroopService(IMapper mapper, ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _mapper = mapper;
        }

        public List<TroopAPIModel> ListOfTroopsMapping(List<Troop> troops)
        {
            var TroopsAPIModels = new List<TroopAPIModel>();

            foreach (var troop in troops)
            {
                var TroopsAPIModel = _mapper.Map<TroopAPIModel>(troop);
                TroopsAPIModels.Add(TroopsAPIModel);
            }
            return TroopsAPIModels;
        }
    }

}

