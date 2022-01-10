using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Kingdom;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Linq;

namespace Naivart.Profiles
{
    public class TroopProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        {
            public TroopProfile()
            {
            CreateMap<Troop, TroopAPIModel>()
            .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.Started_at))
            .ForMember(dest => dest.FinishedAt, opt => opt.MapFrom(src => src.Finished_at))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.TroopType.Level))
            .ForMember(dest => dest.Hp, opt => opt.MapFrom(src => src.TroopType.Hp))
            .ForMember(dest => dest.Attack, opt => opt.MapFrom(src => src.TroopType.Attack))
            .ForMember(dest => dest.Defense, opt => opt.MapFrom(src => src.TroopType.Defense));
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
                .ForMember(dest => dest.Total_defense, opt => opt.MapFrom(src => src.Troops.Sum(d => d.TroopType.Defense)))
                .ForMember(dest => dest.Total_attack, opt => opt.MapFrom(src => src.Troops.Sum(a => a.TroopType.Attack)))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Troops.Sum(ad => ad.TroopType.Attack + ad.TroopType.Defense)));
        }
    }

    public class TroopLostProfile : Profile
    {
        public TroopLostProfile()
        {
            CreateMap<TroopsLost, LostTroopsAPI>();
        }
    }
}
