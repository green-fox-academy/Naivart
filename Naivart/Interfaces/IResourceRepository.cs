using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<List<Resource>> GetResourcesFromIdAsync(long kingdomId);
    }
}
