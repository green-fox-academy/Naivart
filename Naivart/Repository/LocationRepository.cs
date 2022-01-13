using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<bool> IsLocationTakenAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(DbContext.Locations.Any(x => x.CoordinateX == input.CoordinateX && x.CoordinateY == input.CoordinateY));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public async Task<long> GetLocationIdFromCoordinatesAsync(Location model)
        {
            return await Task.FromResult(DbContext.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX && x.CoordinateY == model.CoordinateY).Id);
        }
        public async Task<Location> LocationDefAsync(long kingdomIdDef)
        {
            return await Task.FromResult(DbContext.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdDef).FirstOrDefault().Location);
        }
        public async Task<Location> LocationAttAsync(long kingdomIdAtt)
        {
            return await Task.FromResult(DbContext.Kingdoms.Include(x => x.Location).Where(x => x.Id == kingdomIdAtt).FirstOrDefault().Location);
        }
    }
}
