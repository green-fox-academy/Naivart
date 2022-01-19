using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;


namespace Naivart.Interfaces.ServiceInterfaces
{
    public interface IResourceService
    {
        List<ResourceAPIModel> ListOfResourcesMapping(List<Resource> resources);
    }
}
