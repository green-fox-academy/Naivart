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
        public async Task<BuildingType> GetBuildingTypeAsync(string type, int level)
        {
            return await Task.FromResult(DbContext.BuildingTypes.FirstOrDefault(bt => bt.Type == type && bt.Level == level));
        }
        public async Task<bool> IsEnoughGoldForAsync(int goldAmount, string type, int level)
        {
            try
            {
                return goldAmount >= await Task.FromResult(DbContext.BuildingTypes.FirstOrDefault(bt => bt.Type == type 
                                                           && bt.Level == level).GoldCost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

    }
}
