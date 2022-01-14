using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        Task<List<Building>> GetAllBuildingsThatAreNotDone(long kingdomId);
    }
}
