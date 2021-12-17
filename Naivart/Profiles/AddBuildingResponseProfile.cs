﻿using AutoMapper;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Profiles
{
    public class AddBuildingResponseProfile:Profile
    {
        public AddBuildingResponseProfile()
        {
            CreateMap<Building, AddBuildingForResponse>().ReverseMap();
        }
    }
}