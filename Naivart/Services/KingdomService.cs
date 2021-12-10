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
            var kingdoms = new List<Kingdom>();
            try
            {
                kingdoms = DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .ToList();

                return kingdoms;
            }
            catch
            {
                return kingdoms;
            }
        }
        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                kingdom = DbContext.Kingdoms.Include(k => k.Player).Include(k => k.Location).Include(k => k.Buildings).FirstOrDefault(k => k.Id == id);
                return kingdom;
            }
            catch(Exception)
            {
                return null;
            }
        }

    }
}
