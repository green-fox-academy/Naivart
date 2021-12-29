using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System.Linq;

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

    public class KingdomsLeaderboardProfile : Profile
    {
        public KingdomsLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardKingdomAPIModel>()
                .ForMember(dest => dest.Ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.Kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Buildings_points, opt => opt.MapFrom(src => src.Buildings.Sum(i => i.Level)))
                .ForMember(dest => dest.Troops_points, opt => opt.MapFrom(src => src.Troops.Sum(ad => ad.Attack + ad.Defense)))
                .ForMember(dest => dest.Total_points, opt => opt.MapFrom(src => src.Buildings.Sum(i => i.Level) + src.Troops.Sum(ad => ad.Attack + ad.Defense)));
        }
    }
}
