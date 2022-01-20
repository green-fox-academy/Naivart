using Naivart.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IBuildingTypeRepository : IRepository<BuildingType>
    {
        Task<BuildingType> GetBuildingTypeAsync(string type, int level);
        Task<bool> IsEnoughGoldForAsync(int goldAmount, string type, int level);
    }
}
