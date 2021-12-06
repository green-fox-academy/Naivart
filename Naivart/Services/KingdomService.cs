using Naivart.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class KingdomService
    {
        private ApplicationDbContext DbContext { get; }

        public KingdomService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
