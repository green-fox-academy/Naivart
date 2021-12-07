using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.Entities;
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

        public List<Kingdom> GetAll()
        {
            return DbContext.Kingdoms
                .Include(k => k.Player)
                .Include(k => k.Location)
                .ToList();   
        }
    }
}
