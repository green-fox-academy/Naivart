using AutoMapper;
using Naivart.Models.BuildingTypes;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{
    public class BuildingAddProfile:Profile
    {
        public BuildingAddProfile()
        {
            CreateMap<Building,BuildingModel>().ReverseMap();
        }
    }
}
