using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;

namespace Naivart.Profiles
{
    public class TroopInfoProfile : Profile
    {
        public TroopInfoProfile()
        {
            CreateMap<Troop, TroopsInfo>();
        }
    }
}
