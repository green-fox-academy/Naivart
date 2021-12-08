using Naivart.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naivart.Models.APIModels;

namespace Naivart.Services
{
    public class KingdomService
    {
        private ApplicationDbContext DbContext { get; }

        public KingdomService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void RegisterKingdom(KingdomLocationInput input)
        {
            if (IsCorrectLocationRange(input))
            {
                if (IsLocationFree(input))
                {

                }
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.coordinateX >= 0 && input.coordinateX <= 99 && input.coordinateY >= 0 && input.coordinateY <= 99;
        }

        public bool IsLocationFree(KingdomLocationInput input)
        {
            return !DbContext.Locations.Any(x => x.CoordinateX == input.coordinateX && x.CoordinateY == input.coordinateY);
        }
    }
}
