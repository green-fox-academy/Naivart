using AutoMapper;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;

namespace Naivart.Profiles
{
    public class TroopProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        {
            public TroopProfile()
            {
            CreateMap<Troop, TroopAPIModel>()
            .ForMember(dest => dest.Started_at, opt => opt.MapFrom(src => src.Started_at))
            .ForMember(dest => dest.Finished_at, opt => opt.MapFrom(src => src.Finished_at))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.TroopType.Level))
            .ForMember(dest => dest.Hp, opt => opt.MapFrom(src => src.TroopType.Hp))
            .ForMember(dest => dest.Attack, opt => opt.MapFrom(src => src.TroopType.Attack))
            .ForMember(dest => dest.Defense, opt => opt.MapFrom(src => src.TroopType.Defense));
            }
        }
    
}
