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
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        public ResourceService(IMapper mapper)
        {
            _mapper = mapper;
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
                resourceAPIModels.Add(_mapper.Map<ResourceAPIModel>(resource));
            }
            return resourceAPIModels;
        }
    }
}
