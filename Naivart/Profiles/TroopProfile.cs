using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using Naivart.Models.TroopTypes;
using System.Linq;

namespace Naivart.Profiles
{
    public class TroopProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
    {
        public TroopProfile()
        {
            CreateMap<Troop, TroopAPIModel>();
        }
    }

    public class TroopCreateProfile : Profile
    {
        public TroopCreateProfile()
        {
            CreateMap<Troop, TroopModel>().ReverseMap();
        }
    }

    public class TroopInfoProfile : Profile
    {
        public TroopInfoProfile()
        {
            CreateMap<Troop, TroopInfo>();
        }
    }

    public class TroopLeaderboardProfile : Profile
    {
        public TroopLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardTroopAPIModel>()
                .ForMember(dest => dest.Ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.Kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Total_defense, opt => opt.MapFrom(src => src.Troops.Sum(d => d.Defense)))
                .ForMember(dest => dest.Total_attack, opt => opt.MapFrom(src => src.Troops.Sum(a => a.Attack)))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Troops.Sum(ad => ad.Attack + ad.Defense)));
        }
    }
}
