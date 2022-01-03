using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;

namespace Naivart.Profiles
{
    public class LocationProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
    {
        public LocationProfile()
        {
            CreateMap<Location, LocationAPIModel>();
        }
    }
}
