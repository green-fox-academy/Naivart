using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;

namespace Naivart
{
    public class KingdomProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
    {
        public KingdomProfile()
        {
            CreateMap<Kingdom, KingdomAPIModel>()
           .ForMember(dest => dest.Kingdom_Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.KingdomName, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Ruler, opt => opt.MapFrom(src => src.Player.Username));
        }
    }
}
