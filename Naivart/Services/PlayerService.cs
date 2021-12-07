using Naivart.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class PlayerService
    {
        private ApplicationDbContext DbContext { get; }
        public PlayerService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }


    }
}
