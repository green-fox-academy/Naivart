using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Repository
{
    public class KingdomRepository : Repository<Kingdom>, IKingdomRepository
    {
        public KingdomRepository(ApplicationDbContext context) : base(context)
        {
        }
        public List<Kingdom> GetAll()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .Include(k => k.Buildings)
                    .Include(k => k.Resources)
                    .Include(k => k.Troops)
                    .ThenInclude(k => k.TroopType)
                    .ToList();
            }
            catch
            {
                return kingdoms;
            }
        }
        public Kingdom RenameKingdom(long kingdomId, string newKingdomName)
        {
            Kingdom kingdom = GetById(kingdomId);
            kingdom.Name = newKingdomName;
            DbContext.Update(kingdom);
            DbContext.SaveChanges();
            return kingdom;
        }
    }
}
