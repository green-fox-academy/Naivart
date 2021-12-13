﻿using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class ResourceService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public ResourceService(IMapper mapper, ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _mapper = mapper;
        }

        public List<ResourceAPIModel> ListOfResourcesMapping(List<Resource> resources)
        {
            var resourceAPIModels = new List<ResourceAPIModel>();

            foreach (var resource in resources)
            {
                var resourceAPIModel = _mapper.Map<ResourceAPIModel>(resource);
                resourceAPIModels.Add(resourceAPIModel);
            }
            return resourceAPIModels;
        }
    }
}