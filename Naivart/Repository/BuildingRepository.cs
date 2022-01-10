using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
