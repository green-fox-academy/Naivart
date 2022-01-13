using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class BuildingTypeRepository: Repository<BuildingType>, IBuildingTypeRepository
    {
        public BuildingTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<BuildingType> BuildingTypeIdAsync(long buildingTypeId)
        {
            return await Task.FromResult(DbContext.BuildingTypes.FirstOrDefault(x => x.Id == buildingTypeId + 1));
        }
        public async Task<bool> IsEnoughGoldForAsync(int goldAmount, long buildingTypeId)
        {
            try
            {
                return goldAmount >= await Task.FromResult(DbContext.BuildingTypes.FirstOrDefault
                    (bt => bt.Id == buildingTypeId + 1).GoldCost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

    }
}
