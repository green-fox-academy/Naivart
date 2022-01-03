using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.BuildingTypes;
using Naivart.Models.Entities;
using System.Linq;

namespace Naivart.Profiles
{
    public class BuildingProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
    {
        public BuildingProfile()
        {
            CreateMap<Building, BuildingAPIModel>();
        }
    }

    public class AddBuildingProfile : Profile
    {
        public AddBuildingProfile()
        {
            CreateMap<Building, BuildingModel>().ReverseMap();
        }
    }

    public class AddBuildingResponseProfile : Profile
    {
        public AddBuildingResponseProfile()
        {
            CreateMap<Building, AddBuildingResponse>().ReverseMap();
        }
    }

    public class BuildingLeaderboardProfile : Profile
    {
        public BuildingLeaderboardProfile()
        {
            CreateMap<Kingdom, LeaderboardBuildingAPIModel>()
                .ForMember(dest => dest.Ruler, opt => opt.MapFrom(src => src.Player.Username))
                .ForMember(dest => dest.Kingdom, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Buildings, opt => opt.MapFrom(src => src.Buildings.Count()))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Buildings.Sum(item => item.Level)));
        }
    }
}
