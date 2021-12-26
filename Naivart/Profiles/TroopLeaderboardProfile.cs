using AutoMapper;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System.Linq;

namespace Naivart.Profiles
{
    public class TroopLeaderboardProfile : Profile
    {
        public TroopLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardTroopAPIModel>()
                .ForMember(dest => dest.ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.total_defense, opt => opt.MapFrom(src => src.Troops.Sum(d => d.Defense)))
                .ForMember(dest => dest.total_attack, opt => opt.MapFrom(src => src.Troops.Sum(a => a.Attack)))
                .ForMember(dest => dest.points, opt => opt.MapFrom(src => src.Troops.Sum(ad => ad.Attack + ad.Defense)));
                


        }
    }
}
