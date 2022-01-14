using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Buildings;
using Naivart.Models.Entities;
using Naivart.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(ApplicationDbContext context) : base(context)
        {  
        }

        public async Task<List<Building>> GetAllBuildingsThatAreNotDone(long kingdomId)
        {
            return await Task.FromResult(DbContext.Buildings.Where(x => x.KingdomId == kingdomId
                              && (x.Status == "creating" || x.Status == "upgrading"))
                            .Include(x => x.BuildingType).ToList());
        }
    }
}
