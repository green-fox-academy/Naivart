using Naivart.Database;
using System;
using System.Collections.Generic;
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
        
        public void UpdateResources(long kingdomId)
        {
            var model = DbContext.Resources.Where(x => x.KingdomId == kingdomId).ToList();

            foreach (var item in model)
            {
                item.Amount += CalculateAmount(item.UpdatedAt, item.Generation, out int extra);
                item.UpdatedAt = GetUnixTimeNow() - extra;
                DbContext.Resources.Update(item);
                DbContext.SaveChanges();
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
