using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class ResourceRepository : Repository<Resource>,IResourceRepository
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
