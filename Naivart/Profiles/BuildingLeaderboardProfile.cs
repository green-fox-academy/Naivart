using AutoMapper;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{
    public class BuildingLeaderboardProfile : Profile
    {
        public BuildingLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardBuildingAPIModel>()
                .ForMember(dest => dest.ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.buildings, opt => opt.MapFrom(src => src.Buildings.Count()))
                .ForMember(dest => dest.points, opt => opt.MapFrom(src => src.Buildings.Sum(item => item.Level)));
        }
    }
}
