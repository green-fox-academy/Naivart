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

        public string RegisterKingdom(KingdomLocationInput input, out int status)
        {
            status = 400;
            if (!IsCorrectLocationRange(input))
            {
                return "One or both coordinates are out of valid range (0-99).";
            }
            if (IsLocationTaken(input))
            {
                return "Given coordinates are already taken!";
            }
            status = 200;
            return "ok";
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.coordinateX >= 0 && input.coordinateX <= 99 && input.coordinateY >= 0 && input.coordinateY <= 99;
        }

        public bool IsLocationTaken(KingdomLocationInput input)
        {
            return DbContext.Locations.Any(x => x.CoordinateX == input.coordinateX && x.CoordinateY == input.coordinateY);
        }
    }
}
