using AutoMapper;
using Naivart.Database;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public ResourceService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<ResourceAPIModel> ListOfResourcesMapping(List<Resource> resources)
        {
            var resourceAPIModels = new List<ResourceAPIModel>();
            if (resources is null)
            {
                return resourceAPIModels;
            }

            foreach (var resource in resources)
            {
                var resourceAPIModel = mapper.Map<ResourceAPIModel>(resource);
                resourceAPIModels.Add(resourceAPIModel);
            }
            return resourceAPIModels;
        }
    }
}
