using AutoMapper;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System.Linq;

namespace Naivart.Profiles
{
    public class KingdomsLeaderboardProfile : Profile
    {
        public KingdomsLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardKingdomAPIModel>()
                .ForMember(dest => dest.ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.buildings_points, opt => opt.MapFrom(src => src.Buildings.Sum(i => i.Level)))
                .ForMember(dest => dest.troops_points,
                           opt => opt.MapFrom(src => src.Troops.Sum(ad => ad.Attack + ad.Defense)))
                .ForMember(dest => dest.total_points, opt => opt.MapFrom(src => src.Buildings.Sum(i => i.Level) + src.Troops.Sum(ad => ad.Attack + ad.Defense)));


        }
    }
}
