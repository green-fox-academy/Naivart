using AutoMapper;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;

namespace Naivart.Profiles
{
    public class TroopProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        {
            public TroopProfile()
            {
                CreateMap<Troop, TroopAPIModel>();
            }
        }
    
}
