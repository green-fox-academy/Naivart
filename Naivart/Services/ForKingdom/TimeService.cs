using Naivart.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class TimeService
    {
        private ApplicationDbContext DbContext { get; }
        public TimeService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public long GetUnixTimeNow()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        
        public async Task UpdateResourcesAsync(long kingdomId)
        {
            var resources = await Task.FromResult(DbContext.Resources.Where(x => x.KingdomId == kingdomId).ToList());
            foreach (var resource in resources)
            {
                resource.Amount += CalculateAmount(resource.UpdatedAt, resource.Generation, out int extra);
                resource.UpdatedAt = GetUnixTimeNow() - extra;
                DbContext.Resources.Update(resource);
                await DbContext.SaveChangesAsync();
            }
        }

        public int CalculateAmount(long lastUpdate, int generation, out int extra)
        {
            long secondsSinceLastCheck = GetUnixTimeNow() - lastUpdate;
            extra = Convert.ToInt32(((secondsSinceLastCheck % 600) * generation));
            return Convert.ToInt32(((secondsSinceLastCheck / 600) * generation));
        }
    }
}
