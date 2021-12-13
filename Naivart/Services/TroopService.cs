using Naivart.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class TroopService
    {
        private ApplicationDbContext DbContext { get; }
        public TroopService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
