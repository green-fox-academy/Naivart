using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Resource>> GetResourcesFromIdAsync(long kingdomId)
        {
            return await Task.FromResult(DbContext.Resources.Where(x => x.KingdomId == kingdomId).ToList());
        }        
    }
}
