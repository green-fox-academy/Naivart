using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{
    public class AllPlayerDetailsProfile : Profile
    {
        public AllPlayerDetailsProfile()
        {
            CreateMap<Kingdom, AllPlayerDetails>()
                .ForMember(x => x.kingdom.kingdomId, y => y.MapFrom(y => y.Id))
                .ForMember(x => x.kingdom.kingdomName, y => y.MapFrom(y => y.Name))
                .ForMember(x => x.kingdom.ruler, y => y.MapFrom(y => y.Player.Username))
                .ForMember(x => x.kingdom.population, y => y.MapFrom(y => y.Population))
                .ForMember(x => x.kingdom.location, y => y.MapFrom(y => y.Location));
                //.ForMember(dest => dest.Kingdom_Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.KingdomName, opt => opt.MapFrom(src => src.Name))
                //.ForMember(dest => dest.Ruler, opt => opt.MapFrom(src => src.Player.Username));
        }
    }
}
