using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{ 
        public class TroopProfile : Profile //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        {
            public TroopProfile()
            {
                CreateMap<Troop, TroopAPIModel>();
            }
        }
    
}
