using AutoMapper;
using Naivart.Models.APIModels.Kingdom;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{
    public class TroopLostProfile : Profile
    {
        public TroopLostProfile()
        {
            CreateMap<TroopsLost, LostTroopsAPI>();
        }
    }
}
