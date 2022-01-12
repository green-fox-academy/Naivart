using Naivart.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IBuildingTypeRepository : IRepository<BuildingType>
    {
        Task<BuildingType> BuildingTypeIdAsync(long buildingTypeId);
        Task<bool> IsEnoughGoldForAsync(int goldAmount, long buildingTypeId);
    }
}
